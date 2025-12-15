using BlazorAsetPemda.Data;
using BlazorAsetPemda.Data.Models;
using BlazorAsetPemda.Data.Enums;
using ExcelDataReader;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace BlazorAsetPemda.Services;

/// <summary>
/// Service for importing RefPenyusutan (depreciation reference) data from Excel
/// </summary>
public class RefPenyusutanImportService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RefPenyusutanImportService> _logger;

    public RefPenyusutanImportService(
        ApplicationDbContext context,
        ILogger<RefPenyusutanImportService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Import RefPenyusutan data from Excel file
    /// </summary>
    public async Task<ImportResult> ImportFromExcelAsync(string filePath, string? sheetName = null, int? tahunReferensi = null)
    {
        var result = new ImportResult();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        try
        {
            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            // If sheet name specified, find it
            if (!string.IsNullOrEmpty(sheetName))
            {
                bool found = false;
                do
                {
                    if (reader.Name.Contains(sheetName, StringComparison.OrdinalIgnoreCase))
                    {
                        found = true;
                        break;
                    }
                } while (reader.NextResult());

                if (!found)
                {
                    result.Errors.Add($"Sheet '{sheetName}' not found");
                    return result;
                }
            }

            _logger.LogInformation("Reading sheet: {SheetName}, Rows: {Rows}, Cols: {Cols}",
                reader.Name, reader.RowCount, reader.FieldCount);

            // Read header row to detect column mapping
            if (!reader.Read())
            {
                result.Errors.Add("File is empty");
                return result;
            }

            // Detect column indices from headers
            var columnMap = DetectColumns(reader);
            _logger.LogInformation("Detected columns: {Columns}", string.Join(", ", columnMap.Select(kv => $"{kv.Key}={kv.Value}")));

            // Read data rows
            int rowNumber = 1;
            while (reader.Read())
            {
                rowNumber++;
                try
                {
                    // Skip empty rows
                    if (IsEmptyRow(reader)) continue;

                    var kodeBarang = GetStringValue(reader, columnMap.GetValueOrDefault("KodeBarang", 0));
                    var namaBarang = GetStringValue(reader, columnMap.GetValueOrDefault("NamaBarang", 1));

                    // Skip if no kode barang
                    if (string.IsNullOrWhiteSpace(kodeBarang)) continue;

                    var masaManfaat = GetIntValue(reader, columnMap.GetValueOrDefault("MasaManfaat", -1));
                    var persenPenyusutan = GetDecimalValue(reader, columnMap.GetValueOrDefault("PersenPenyusutan", -1));
                    var nilaiResidu = GetDecimalValue(reader, columnMap.GetValueOrDefault("NilaiResidu", -1));
                    var kelompok = GetStringValue(reader, columnMap.GetValueOrDefault("Kelompok", -1));

                    // Determine KIB type from kode barang
                    var kibType = DetermineKIBType(kodeBarang);

                    // Calculate masa manfaat in months if years provided
                    int? masaManfaatBulan = masaManfaat.HasValue ? masaManfaat.Value * 12 : null;

                    // Calculate persen if masa manfaat provided but persen not
                    if (masaManfaat.HasValue && masaManfaat.Value > 0 && !persenPenyusutan.HasValue)
                    {
                        persenPenyusutan = 100m / masaManfaat.Value;
                    }

                    // Check if exists
                    var existing = await _context.RefPenyusutans
                        .FirstOrDefaultAsync(r => r.KodeBarang == kodeBarang);

                    if (existing != null)
                    {
                        // Update existing
                        existing.NamaBarang = namaBarang ?? existing.NamaBarang;
                        existing.KIBType = kibType;
                        existing.MasaManfaat = masaManfaat ?? existing.MasaManfaat;
                        existing.MasaManfaatBulan = masaManfaatBulan ?? existing.MasaManfaatBulan;
                        existing.PersenPenyusutan = persenPenyusutan ?? existing.PersenPenyusutan;
                        existing.NilaiResidu = nilaiResidu ?? existing.NilaiResidu;
                        existing.Kelompok = kelompok ?? existing.Kelompok;
                        existing.TahunReferensi = tahunReferensi ?? existing.TahunReferensi;
                        existing.UpdatedAt = DateTime.UtcNow;
                        result.UpdatedCount++;
                    }
                    else
                    {
                        // Create new
                        var refPenyusutan = new RefPenyusutan
                        {
                            KodeBarang = kodeBarang,
                            NamaBarang = namaBarang ?? "",
                            KIBType = kibType,
                            MasaManfaat = masaManfaat,
                            MasaManfaatBulan = masaManfaatBulan,
                            PersenPenyusutan = persenPenyusutan,
                            NilaiResidu = nilaiResidu ?? 0,
                            MetodePenyusutan = "Garis Lurus",
                            Kelompok = kelompok,
                            TahunReferensi = tahunReferensi,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        };
                        _context.RefPenyusutans.Add(refPenyusutan);
                        result.SuccessCount++;
                    }

                    // Save every 100 rows
                    if ((result.SuccessCount + result.UpdatedCount) % 100 == 0)
                    {
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Row {rowNumber}: {ex.Message}");
                    _logger.LogError(ex, "Error importing row {Row}", rowNumber);
                }
            }

            await _context.SaveChangesAsync();
            result.IsSuccess = result.Errors.Count == 0 || (result.SuccessCount + result.UpdatedCount) > 0;
        }
        catch (Exception ex)
        {
            result.Errors.Add($"General error: {ex.Message}");
            _logger.LogError(ex, "Error importing RefPenyusutan");
        }

        return result;
    }

    /// <summary>
    /// Preview Excel file structure and first N rows
    /// </summary>
    public List<Dictionary<string, object?>> PreviewExcel(string filePath, int maxRows = 20)
    {
        var result = new List<Dictionary<string, object?>>();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        try
        {
            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            // Read headers
            if (!reader.Read()) return result;

            var headers = new List<string>();
            for (int c = 0; c < reader.FieldCount; c++)
            {
                var header = reader.GetValue(c)?.ToString()?.Trim() ?? $"Col{c}";
                headers.Add(header);
            }

            // Read data rows
            while (reader.Read() && result.Count < maxRows)
            {
                if (IsEmptyRow(reader)) continue;

                var row = new Dictionary<string, object?>();
                for (int c = 0; c < Math.Min(headers.Count, reader.FieldCount); c++)
                {
                    row[headers[c]] = reader.GetValue(c);
                }
                result.Add(row);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error previewing Excel file");
        }

        return result;
    }

    /// <summary>
    /// Get Excel headers for column mapping display
    /// </summary>
    public List<string> GetExcelHeaders(string filePath)
    {
        var headers = new List<string>();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        try
        {
            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            if (reader.Read())
            {
                for (int c = 0; c < reader.FieldCount; c++)
                {
                    var header = reader.GetValue(c)?.ToString()?.Trim() ?? "";
                    if (!string.IsNullOrEmpty(header))
                        headers.Add($"[{c}] {header}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading Excel headers");
        }

        return headers;
    }

    /// <summary>
    /// Get count of RefPenyusutan records
    /// </summary>
    public async Task<int> GetCountAsync()
    {
        return await _context.RefPenyusutans.CountAsync();
    }

    /// <summary>
    /// Get paged RefPenyusutan records
    /// </summary>
    public async Task<List<RefPenyusutan>> GetPagedAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.RefPenyusutans.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(r =>
                r.KodeBarang.Contains(searchTerm) ||
                r.NamaBarang.Contains(searchTerm));
        }

        return await query
            .OrderBy(r => r.KodeBarang)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// Clear all RefPenyusutan data
    /// </summary>
    public async Task<bool> ClearAllAsync()
    {
        try
        {
            await _context.RefPenyusutans.ExecuteDeleteAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing RefPenyusutan data");
            return false;
        }
    }

    private Dictionary<string, int> DetectColumns(IExcelDataReader reader)
    {
        var map = new Dictionary<string, int>();

        for (int c = 0; c < reader.FieldCount; c++)
        {
            var header = reader.GetValue(c)?.ToString()?.ToLower().Trim() ?? "";

            if (header.Contains("kode") && header.Contains("barang") || header == "kode_barang" || header == "kdbarang")
                map["KodeBarang"] = c;
            else if (header.Contains("nama") && header.Contains("barang") || header == "nm_barang" || header == "nmbarang" || header == "uraian")
                map["NamaBarang"] = c;
            else if (header.Contains("masa") && header.Contains("manfaat") || header == "umur" || header == "tahun")
                map["MasaManfaat"] = c;
            else if (header.Contains("persen") || header.Contains("%") || header == "tarif")
                map["PersenPenyusutan"] = c;
            else if (header.Contains("residu") || header.Contains("sisa"))
                map["NilaiResidu"] = c;
            else if (header.Contains("kelompok") || header.Contains("kategori") || header.Contains("golongan"))
                map["Kelompok"] = c;
        }

        // Default mapping if not detected
        if (!map.ContainsKey("KodeBarang")) map["KodeBarang"] = 0;
        if (!map.ContainsKey("NamaBarang")) map["NamaBarang"] = 1;

        return map;
    }

    private KIBType? DetermineKIBType(string kodeBarang)
    {
        if (string.IsNullOrEmpty(kodeBarang)) return null;

        var parts = kodeBarang.Split('.');
        if (parts.Length < 1) return null;

        // Try to get jenis from first or third segment
        var jenis = parts[0].TrimStart('0');
        if (parts.Length >= 3 && int.TryParse(parts[2].TrimStart('0'), out int jenisFromThird))
        {
            jenis = jenisFromThird.ToString();
        }

        return jenis switch
        {
            "1" => KIBType.A,  // Tanah
            "2" => KIBType.B,  // Peralatan dan Mesin
            "3" => KIBType.C,  // Gedung dan Bangunan
            "4" => KIBType.D,  // Jalan, Jaringan, Irigasi
            "5" => KIBType.E,  // Aset Tetap Lainnya
            "6" => KIBType.F,  // Konstruksi Dalam Pengerjaan
            _ => null
        };
    }

    private bool IsEmptyRow(IExcelDataReader reader)
    {
        for (int c = 0; c < Math.Min(reader.FieldCount, 5); c++)
        {
            var val = reader.GetValue(c);
            if (val != null && !string.IsNullOrWhiteSpace(val.ToString()))
                return false;
        }
        return true;
    }

    private string? GetStringValue(IExcelDataReader reader, int index)
    {
        if (index < 0 || index >= reader.FieldCount) return null;
        return reader.GetValue(index)?.ToString()?.Trim();
    }

    private int? GetIntValue(IExcelDataReader reader, int index)
    {
        if (index < 0 || index >= reader.FieldCount) return null;
        var value = reader.GetValue(index);
        if (value == null) return null;
        if (value is double d) return (int)d;
        if (int.TryParse(value.ToString(), out int result)) return result;
        return null;
    }

    private decimal? GetDecimalValue(IExcelDataReader reader, int index)
    {
        if (index < 0 || index >= reader.FieldCount) return null;
        var value = reader.GetValue(index);
        if (value == null) return null;
        if (value is double d) return (decimal)d;
        if (decimal.TryParse(value.ToString(), out decimal result)) return result;
        return null;
    }
}
