using BlazorAsetPemda.Data;
using BlazorAsetPemda.Data.Models;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;

namespace BlazorAsetPemda.Services;

/// <summary>
/// Service for importing BMD Reference Codes from Permendagri Excel file
/// </summary>
public class BMDReferenceImportService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BMDReferenceImportService> _logger;

    public BMDReferenceImportService(
        ApplicationDbContext context,
        ILogger<BMDReferenceImportService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Import BMD reference codes from Excel file
    /// </summary>
    /// <param name="filePath">Path to the Permendagri Excel file</param>
    /// <param name="sheetName">Sheet name to import (default: "SEMUA KODE")</param>
    /// <returns>Import result with success count and errors</returns>
    public async Task<ImportResult> ImportFromExcelAsync(string filePath, string sheetName = "SEMUA KODE")
    {
        var result = new ImportResult();

        try
        {
            // Set EPPlus license
            ExcelPackage.License.SetNonCommercialPersonal("BlazorAsetPemda");

            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets[sheetName];

            if (worksheet == null)
            {
                result.Errors.Add($"Sheet '{sheetName}' not found in the Excel file");
                return result;
            }

            if (worksheet.Dimension == null)
            {
                result.Errors.Add($"Sheet '{sheetName}' is empty");
                return result;
            }

            // Clear existing data (optional - you may want to ask user first)
            // await _context.KodeRekeningBMDs.ExecuteDeleteAsync();

            // Start from row 4 (header is at row 4, data starts at row 6)
            // Row 4 contains: AKUN, KELOMPOK, JENIS, OBJEK, RINCIAN OBJEK, SUB RINCIAN OBJEK, SUB - SUB RINCIAN OBJEK, URAIAN
            for (int row = 6; row <= worksheet.Dimension.End.Row; row++)
            {
                try
                {
                    var akun = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                    var kelompok = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                    var jenis = worksheet.Cells[row, 3].Value?.ToString()?.Trim();
                    var objek = worksheet.Cells[row, 4].Value?.ToString()?.Trim();
                    var rincianObjek = worksheet.Cells[row, 5].Value?.ToString()?.Trim();
                    var subRincianObjek = worksheet.Cells[row, 6].Value?.ToString()?.Trim();
                    var subSubRincianObjek = worksheet.Cells[row, 7].Value?.ToString()?.Trim();
                    var uraian = worksheet.Cells[row, 8].Value?.ToString()?.Trim();

                    // Skip if no description or all codes are empty
                    if (string.IsNullOrWhiteSpace(uraian) ||
                        (string.IsNullOrWhiteSpace(akun) &&
                         string.IsNullOrWhiteSpace(kelompok) &&
                         string.IsNullOrWhiteSpace(jenis)))
                    {
                        continue;
                    }

                    // Build the full code in format xx.xx.xx.xx.xx.xx.xx
                    var codeParts = new List<string>();
                    if (!string.IsNullOrWhiteSpace(akun)) codeParts.Add(akun);
                    if (!string.IsNullOrWhiteSpace(kelompok)) codeParts.Add(kelompok);
                    if (!string.IsNullOrWhiteSpace(jenis)) codeParts.Add(jenis);
                    if (!string.IsNullOrWhiteSpace(objek)) codeParts.Add(objek);
                    if (!string.IsNullOrWhiteSpace(rincianObjek)) codeParts.Add(rincianObjek);
                    if (!string.IsNullOrWhiteSpace(subRincianObjek)) codeParts.Add(subRincianObjek);
                    if (!string.IsNullOrWhiteSpace(subSubRincianObjek)) codeParts.Add(subSubRincianObjek);

                    var kodeRekening = string.Join(".", codeParts);

                    if (string.IsNullOrWhiteSpace(kodeRekening))
                    {
                        continue;
                    }

                    // Determine asset category from Jenis level
                    string? kategoriAset = null;
                    if (!string.IsNullOrWhiteSpace(jenis))
                    {
                        kategoriAset = jenis switch
                        {
                            "1" => "Tanah",
                            "2" => "Peralatan dan Mesin",
                            "3" => "Gedung dan Bangunan",
                            "4" => "Jalan, Jaringan dan Irigasi",
                            "5" => "Aset Tetap Lainnya",
                            "6" => "Konstruksi Dalam Pengerjaan",
                            "7" => "Akumulasi Penyusutan",
                            _ => null
                        };
                    }

                    // Check if record already exists
                    var existing = await _context.KodeRekeningBMDs
                        .FirstOrDefaultAsync(k => k.KodeRekening == kodeRekening);

                    if (existing != null)
                    {
                        // Update existing record
                        existing.Akun = akun;
                        existing.Kelompok = kelompok;
                        existing.Jenis = jenis;
                        existing.Objek = objek;
                        existing.RincianObjek = rincianObjek;
                        existing.SubRincianObjek = subRincianObjek;
                        existing.SubSubRincianObjek = subSubRincianObjek;
                        existing.Uraian = uraian ?? "";
                        existing.KategoriAset = kategoriAset;
                        existing.UpdatedAt = DateTime.UtcNow;

                        result.UpdatedCount++;
                    }
                    else
                    {
                        // Create new record
                        var kodeRekeningBMD = new KodeRekeningBMD
                        {
                            KodeRekening = kodeRekening,
                            Akun = akun,
                            Kelompok = kelompok,
                            Jenis = jenis,
                            Objek = objek,
                            RincianObjek = rincianObjek,
                            SubRincianObjek = subRincianObjek,
                            SubSubRincianObjek = subSubRincianObjek,
                            Uraian = uraian ?? "",
                            KategoriAset = kategoriAset,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        };

                        _context.KodeRekeningBMDs.Add(kodeRekeningBMD);
                        result.SuccessCount++;
                    }

                    // Save every 100 rows to avoid memory issues
                    if ((result.SuccessCount + result.UpdatedCount) % 100 == 0)
                    {
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Row {row}: {ex.Message}");
                    _logger.LogError(ex, "Error importing row {Row}", row);
                }
            }

            // Save any remaining changes
            await _context.SaveChangesAsync();

            result.IsSuccess = result.Errors.Count == 0 || (result.SuccessCount + result.UpdatedCount) > 0;
        }
        catch (Exception ex)
        {
            result.Errors.Add($"General error: {ex.Message}");
            _logger.LogError(ex, "Error importing BMD reference codes");
        }

        return result;
    }

    /// <summary>
    /// Get total count of BMD reference codes in database
    /// </summary>
    public async Task<int> GetTotalCountAsync()
    {
        return await _context.KodeRekeningBMDs.CountAsync();
    }

    /// <summary>
    /// Clear all BMD reference codes
    /// </summary>
    public async Task<bool> ClearAllAsync()
    {
        try
        {
            await _context.KodeRekeningBMDs.ExecuteDeleteAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing BMD reference codes");
            return false;
        }
    }
}

/// <summary>
/// Result of import operation
/// </summary>
public class ImportResult
{
    public bool IsSuccess { get; set; }
    public int SuccessCount { get; set; }
    public int UpdatedCount { get; set; }
    public List<string> Errors { get; set; } = new();
    public Guid? ImportBatchId { get; set; }

    public string GetSummary()
    {
        if (IsSuccess)
        {
            return $"Import successful: {SuccessCount} new records, {UpdatedCount} updated records";
        }
        else
        {
            return $"Import failed with {Errors.Count} errors. {SuccessCount} new records, {UpdatedCount} updated records";
        }
    }
}
