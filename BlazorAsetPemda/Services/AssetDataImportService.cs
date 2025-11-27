using BlazorAsetPemda.Data;
using BlazorAsetPemda.Data.Models;
using ExcelDataReader;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace BlazorAsetPemda.Services;

/// <summary>
/// Service for importing Asset data from Excel files (EXCELL 7 format)
/// </summary>
public class AssetDataImportService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AssetDataImportService> _logger;

    public AssetDataImportService(
        ApplicationDbContext context,
        ILogger<AssetDataImportService> logger)
    {
        _context = context;
        _logger = logger;

        // Register encoding provider for ExcelDataReader
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    /// <summary>
    /// Import asset data from Excel file (.xls or .xlsx)
    /// </summary>
    /// <param name="filePath">Path to the Excel file</param>
    /// <param name="sheetName">Sheet name to import (default: "Input Data")</param>
    /// <param name="importedBy">User who is importing the data</param>
    /// <returns>Import result with success count and errors</returns>
    public async Task<ImportResult> ImportFromExcelAsync(string filePath, string sheetName = "Input Data", string? importedBy = null)
    {
        var result = new ImportResult();
        var importBatchId = Guid.NewGuid();

        try
        {
            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream);

            // Find the sheet
            bool sheetFound = false;
            do
            {
                if (reader.Name.Contains(sheetName, StringComparison.OrdinalIgnoreCase))
                {
                    sheetFound = true;
                    break;
                }
            } while (reader.NextResult());

            if (!sheetFound)
            {
                result.Errors.Add($"Sheet '{sheetName}' not found in the Excel file");
                return result;
            }

            // Skip to row 10 where the actual column headers are
            // (Rows 1-9 contain titles and merged headers)
            for (int i = 0; i < 9; i++)
            {
                if (!reader.Read())
                {
                    result.Errors.Add("Excel file has fewer than 10 rows");
                    return result;
                }
            }

            // Read header row (row 10)
            if (!reader.Read())
            {
                result.Errors.Add("Cannot read header row");
                return result;
            }

            // Process data rows (starting from row 11)
            int rowNumber = 11;
            while (reader.Read())
            {
                try
                {
                    // Skip empty rows (check if first few columns are all null)
                    if (IsEmptyRow(reader))
                    {
                        rowNumber++;
                        continue;
                    }

                    var assetData = new AssetImportData
                    {
                        // Column mappings based on row 10 headers
                        Urut = GetIntValue(reader, 0),
                        Kd_UPB = GetStringValue(reader, 1),
                        Nm_UPB = GetStringValue(reader, 2),
                        No_Ruang = GetStringValue(reader, 3),
                        Nm_Ruang = GetStringValue(reader, 4),
                        Jns_Aset = GetStringValue(reader, 5),
                        Kode_Barang = GetStringValue(reader, 6),
                        Nm_Barang = GetStringValue(reader, 7),
                        Jn_Kap = GetStringValue(reader, 8),
                        No_Kap = GetStringValue(reader, 9),
                        Reg_Kap = GetStringValue(reader, 10),
                        Tgl_Perolehan = GetDateValue(reader, 11),
                        Keterangan = GetStringValue(reader, 12),
                        Kd_Posisi = GetStringValue(reader, 13),
                        Pemilik = GetStringValue(reader, 14),
                        Asal_Usul = GetStringValue(reader, 15),
                        Kon_b = GetStringValue(reader, 16),
                        Jl_Barang = GetIntValue(reader, 17),
                        Harga = GetDecimalValue(reader, 18),
                        Harga_Total = GetDecimalValue(reader, 19),
                        Masa_Manfaat = GetIntValue(reader, 20),
                        KDP = GetStringValue(reader, 21),
                        Intra_Ekstra = GetStringValue(reader, 22),
                        Alamat = GetStringValue(reader, 23),
                        Panjang = GetDecimalValue(reader, 24),
                        Lebar = GetDecimalValue(reader, 25),
                        Luas = GetDecimalValue(reader, 26),
                        Hak_Tanah = GetStringValue(reader, 27),
                        Penggunaan_Merk = GetStringValue(reader, 28),
                        Type_Panjang = GetStringValue(reader, 29),
                        CC_Lebar = GetStringValue(reader, 30),
                        Bahan = GetStringValue(reader, 31),
                        Tgl_Dokumen = GetDateValue(reader, 32),
                        No_Dokumen = GetStringValue(reader, 33),
                        Status_Tanah = GetStringValue(reader, 34),
                        Bertingkat = GetStringValue(reader, 35),
                        Beton = GetStringValue(reader, 36),
                        No_Rangka = GetStringValue(reader, 37),
                        No_Mesin = GetStringValue(reader, 38),
                        No_Polisi = GetStringValue(reader, 39),
                        No_BPKB = GetStringValue(reader, 40),
                        Nm_File = GetStringValue(reader, 41),
                        Proc_Id = GetStringValue(reader, 42),

                        // Metadata
                        ImportBatchId = importBatchId,
                        SourceFileName = Path.GetFileName(filePath),
                        ImportedAt = DateTime.UtcNow,
                        ImportedBy = importedBy,
                        IsProcessed = false
                    };

                    _context.AssetImportDatas.Add(assetData);
                    result.SuccessCount++;

                    // Save every 100 rows to avoid memory issues
                    if (result.SuccessCount % 100 == 0)
                    {
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Row {rowNumber}: {ex.Message}");
                    _logger.LogError(ex, "Error importing row {Row}", rowNumber);
                }

                rowNumber++;
            }

            // Save any remaining changes
            await _context.SaveChangesAsync();

            result.IsSuccess = result.Errors.Count == 0 || result.SuccessCount > 0;
            result.ImportBatchId = importBatchId;
        }
        catch (Exception ex)
        {
            result.Errors.Add($"General error: {ex.Message}");
            _logger.LogError(ex, "Error importing asset data");
        }

        return result;
    }

    /// <summary>
    /// Check if a row is empty (all values are null or whitespace)
    /// </summary>
    private bool IsEmptyRow(IExcelDataReader reader)
    {
        // Check first 8 columns - if all are empty, consider row empty
        for (int i = 0; i < Math.Min(8, reader.FieldCount); i++)
        {
            var value = reader.GetValue(i);
            if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Get string value from Excel cell
    /// </summary>
    private string? GetStringValue(IExcelDataReader reader, int columnIndex)
    {
        if (columnIndex >= reader.FieldCount) return null;
        var value = reader.GetValue(columnIndex);
        return value?.ToString()?.Trim();
    }

    /// <summary>
    /// Get integer value from Excel cell
    /// </summary>
    private int? GetIntValue(IExcelDataReader reader, int columnIndex)
    {
        if (columnIndex >= reader.FieldCount) return null;
        var value = reader.GetValue(columnIndex);
        if (value == null) return null;

        if (value is int intValue) return intValue;
        if (value is double doubleValue) return (int)doubleValue;
        if (int.TryParse(value.ToString(), out int result)) return result;

        return null;
    }

    /// <summary>
    /// Get decimal value from Excel cell
    /// </summary>
    private decimal? GetDecimalValue(IExcelDataReader reader, int columnIndex)
    {
        if (columnIndex >= reader.FieldCount) return null;
        var value = reader.GetValue(columnIndex);
        if (value == null) return null;

        if (value is decimal decimalValue) return decimalValue;
        if (value is double doubleValue) return (decimal)doubleValue;
        if (decimal.TryParse(value.ToString(), out decimal result)) return result;

        return null;
    }

    /// <summary>
    /// Get date value from Excel cell
    /// </summary>
    private DateTime? GetDateValue(IExcelDataReader reader, int columnIndex)
    {
        if (columnIndex >= reader.FieldCount) return null;
        var value = reader.GetValue(columnIndex);
        if (value == null) return null;

        if (value is DateTime dateValue) return dateValue;
        if (DateTime.TryParse(value.ToString(), out DateTime result)) return result;

        return null;
    }

    /// <summary>
    /// Get total count of imported asset data in database
    /// </summary>
    public async Task<int> GetTotalCountAsync()
    {
        return await _context.AssetImportDatas.CountAsync();
    }

    /// <summary>
    /// Get import batches
    /// </summary>
    public async Task<List<ImportBatchInfo>> GetImportBatchesAsync()
    {
        return await _context.AssetImportDatas
            .GroupBy(a => a.ImportBatchId)
            .Select(g => new ImportBatchInfo
            {
                BatchId = g.Key ?? Guid.Empty,
                Count = g.Count(),
                FileName = g.First().SourceFileName,
                ImportedBy = g.First().ImportedBy,
                ImportedAt = g.First().ImportedAt
            })
            .OrderByDescending(b => b.ImportedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Delete import batch
    /// </summary>
    public async Task<bool> DeleteImportBatchAsync(Guid batchId)
    {
        try
        {
            await _context.AssetImportDatas
                .Where(a => a.ImportBatchId == batchId)
                .ExecuteDeleteAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting import batch {BatchId}", batchId);
            return false;
        }
    }

    /// <summary>
    /// Clear all asset import data
    /// </summary>
    public async Task<bool> ClearAllAsync()
    {
        try
        {
            await _context.AssetImportDatas.ExecuteDeleteAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing asset import data");
            return false;
        }
    }
}

/// <summary>
/// Import batch information
/// </summary>
public class ImportBatchInfo
{
    public Guid BatchId { get; set; }
    public int Count { get; set; }
    public string? FileName { get; set; }
    public string? ImportedBy { get; set; }
    public DateTime ImportedAt { get; set; }
}
