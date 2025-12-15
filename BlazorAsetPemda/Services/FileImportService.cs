using BlazorAsetPemda.Data;
using BlazorAsetPemda.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorAsetPemda.Services;

/// <summary>
/// Service for managing import files (upload, storage, metadata)
/// </summary>
public class FileImportService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<FileImportService> _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly string _uploadPath;

    public FileImportService(
        ApplicationDbContext context,
        ILogger<FileImportService> logger,
        IWebHostEnvironment environment)
    {
        _context = context;
        _logger = logger;
        _environment = environment;
        _uploadPath = Path.Combine(_environment.ContentRootPath, "Uploads", "Imports");

        // Ensure upload directory exists
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    /// <summary>
    /// Save uploaded file and create metadata record
    /// </summary>
    public async Task<ImportFile> SaveFileAsync(Stream fileStream, string fileName, string importType, string? uploadedBy)
    {
        try
        {
            // Generate unique filename
            var extension = Path.GetExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var typePath = Path.Combine(_uploadPath, importType);

            if (!Directory.Exists(typePath))
            {
                Directory.CreateDirectory(typePath);
            }

            var filePath = Path.Combine(typePath, uniqueFileName);

            // Save file to disk
            using (var outputStream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(outputStream);
            }

            // Create metadata record
            var importFile = new ImportFile
            {
                FileName = fileName,
                StoredFileName = Path.Combine(importType, uniqueFileName),
                FileSize = new FileInfo(filePath).Length,
                ImportType = importType,
                UploadedAt = DateTime.Now,
                UploadedBy = uploadedBy,
                Status = ImportStatus.Pending
            };

            _context.ImportFiles.Add(importFile);
            await _context.SaveChangesAsync();

            _logger.LogInformation("File saved: {FileName} -> {StoredFileName}", fileName, uniqueFileName);
            return importFile;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving file {FileName}", fileName);
            throw;
        }
    }

    /// <summary>
    /// Get full path to stored file
    /// </summary>
    public string GetFilePath(ImportFile importFile)
    {
        return Path.Combine(_uploadPath, importFile.StoredFileName);
    }

    /// <summary>
    /// Get all import files by type
    /// </summary>
    public async Task<List<ImportFile>> GetFilesByTypeAsync(string importType)
    {
        return await _context.ImportFiles
            .Where(f => f.ImportType == importType)
            .OrderByDescending(f => f.UploadedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Get import file by ID
    /// </summary>
    public async Task<ImportFile?> GetByIdAsync(int id)
    {
        return await _context.ImportFiles.FindAsync(id);
    }

    /// <summary>
    /// Delete import file and related data
    /// </summary>
    public async Task<bool> DeleteFileAsync(int fileId)
    {
        try
        {
            var importFile = await _context.ImportFiles.FindAsync(fileId);
            if (importFile == null) return false;

            // Delete related import data
            await _context.AssetImportDatas
                .Where(a => a.ImportFileId == fileId)
                .ExecuteDeleteAsync();

            // Delete physical file
            var filePath = GetFilePath(importFile);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // Delete metadata record
            _context.ImportFiles.Remove(importFile);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted import file: {FileId} - {FileName}", fileId, importFile.FileName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {FileId}", fileId);
            return false;
        }
    }

    /// <summary>
    /// Update import file status
    /// </summary>
    public async Task UpdateStatusAsync(int fileId, string status, int? importedRows = null, int? errorRows = null, int? verifiedRows = null)
    {
        var importFile = await _context.ImportFiles.FindAsync(fileId);
        if (importFile == null) return;

        importFile.Status = status;

        if (importedRows.HasValue)
            importFile.ImportedRows = importedRows.Value;
        if (errorRows.HasValue)
            importFile.ErrorRows = errorRows.Value;
        if (verifiedRows.HasValue)
            importFile.VerifiedRows = verifiedRows.Value;

        if (status == ImportStatus.Imported)
            importFile.ImportedAt = DateTime.Now;
        if (status == ImportStatus.Verified)
            importFile.VerifiedAt = DateTime.Now;
        if (status == ImportStatus.Completed)
            importFile.CompletedAt = DateTime.Now;

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Get import data for a specific file
    /// </summary>
    public async Task<List<AssetImportData>> GetImportDataAsync(int fileId, int page = 1, int pageSize = 50)
    {
        return await _context.AssetImportDatas
            .Where(a => a.ImportFileId == fileId)
            .OrderBy(a => a.RowNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// Get total count of import data for a file
    /// </summary>
    public async Task<int> GetImportDataCountAsync(int fileId)
    {
        return await _context.AssetImportDatas
            .Where(a => a.ImportFileId == fileId)
            .CountAsync();
    }

    /// <summary>
    /// Verify import data (check for errors)
    /// </summary>
    public async Task<VerificationResult> VerifyImportDataAsync(int fileId)
    {
        var result = new VerificationResult();
        var importData = await _context.AssetImportDatas
            .Where(a => a.ImportFileId == fileId)
            .ToListAsync();

        // Pre-load reference data for validation
        var kodeBarangList = await _context.KodeBarangs.ToListAsync();
        var kontrakList = await _context.Kontraks.Select(k => k.NomorKontrak).ToListAsync();
        var hasMasterKodeBarang = kodeBarangList.Any();

        // For kapitalisasi validation - get existing assets by register
        var existingAssets = await _context.Assets
            .Select(a => new { a.NomorRegister, a.NamaBarang })
            .ToListAsync();

        foreach (var data in importData)
        {
            var errors = new List<string>();

            // 1. Validate required fields
            if (string.IsNullOrWhiteSpace(data.Kode_Barang))
                errors.Add("Kode Barang kosong");
            if (string.IsNullOrWhiteSpace(data.Nm_Barang))
                errors.Add("Nama Barang kosong");
            if (!data.Harga.HasValue || data.Harga <= 0)
                errors.Add("Harga tidak valid");
            if (!data.Tgl_Perolehan.HasValue)
                errors.Add("Tanggal Perolehan kosong");

            // 2. Validate Kode Barang format and match with master data
            if (!string.IsNullOrWhiteSpace(data.Kode_Barang))
            {
                var parts = data.Kode_Barang.Split('.');
                if (parts.Length < 4)
                    errors.Add("Format Kode Barang tidak valid");

                // Only check master if master data exists
                if (hasMasterKodeBarang)
                {
                    // Try exact match first
                    var masterKodeBarang = kodeBarangList.FirstOrDefault(k => k.KodeBarang1 == data.Kode_Barang);

                    // If no exact match, try flexible matching
                    if (masterKodeBarang == null)
                    {
                        // Try partial match (master ends with import code)
                        masterKodeBarang = kodeBarangList.FirstOrDefault(k =>
                            k.KodeBarang1.EndsWith("." + data.Kode_Barang) ||
                            k.KodeBarang1.EndsWith(data.Kode_Barang));
                    }

                    // Try matching by normalized segments (strip leading zeros)
                    if (masterKodeBarang == null)
                    {
                        var importParts = data.Kode_Barang.Split('.');
                        var normalizedImport = string.Join(".", importParts.Select(p => p.TrimStart('0')));

                        masterKodeBarang = kodeBarangList.FirstOrDefault(k =>
                        {
                            // Check if master contains normalized import code
                            var masterParts = k.KodeBarang1.Split('.');
                            var normalizedMaster = string.Join(".", masterParts.Select(p => p.TrimStart('0').TrimStart('\'')));
                            return normalizedMaster.Contains(normalizedImport) ||
                                   normalizedMaster.EndsWith(normalizedImport);
                        });
                    }

                    // Note: If still not found, log a warning but don't block
                    // SIMDA and Permendagri use incompatible code formats
                    if (masterKodeBarang == null)
                    {
                        // Check if format appears to be SIMDA (2-digit segments starting with jenis)
                        var firstPart = parts.Length > 0 ? parts[0] : "";
                        var isSimdaFormat = parts.Length == 5 && int.TryParse(firstPart, out int jenis) && jenis >= 1 && jenis <= 6;

                        if (!isSimdaFormat)
                        {
                            errors.Add("Kode Barang tidak ditemukan di master");
                        }
                        // If SIMDA format, skip master validation (incompatible with Permendagri)
                    }
                }
            }

            // 3. Validate Jumlah Barang (must be positive integer)
            if (!data.Jl_Barang.HasValue || data.Jl_Barang <= 0)
                errors.Add("Jumlah Barang harus angka positif");

            // 4. Ruangan validation - relaxed, allow partial data
            // (No strict validation - either or both can be empty/filled)

            // 5. Validate Kontrak reference (if kapitalisasi and kontrak provided)
            if (!string.IsNullOrWhiteSpace(data.No_Kap) && kontrakList.Any())
            {
                if (!kontrakList.Any(k => k != null && k.Contains(data.No_Kap, StringComparison.OrdinalIgnoreCase)))
                {
                    errors.Add($"Kontrak '{data.No_Kap}' belum diimport");
                }
            }

            // 6. Validate calculation: Jumlah × Harga = Harga_Total
            if (data.Jl_Barang.HasValue && data.Harga.HasValue && data.Harga_Total.HasValue)
            {
                var calculatedTotal = data.Jl_Barang.Value * data.Harga.Value;
                if (Math.Abs(calculatedTotal - data.Harga_Total.Value) > 1)
                {
                    errors.Add($"Jumlah × Harga ({calculatedTotal:N0}) ≠ Total ({data.Harga_Total.Value:N0})");
                }
            }

            // 7. Validate character length for key fields
            if (!string.IsNullOrEmpty(data.Kode_Barang) && data.Kode_Barang.Length > 100)
                errors.Add("Kode Barang melebihi 100 karakter");
            if (!string.IsNullOrEmpty(data.Nm_Barang) && data.Nm_Barang.Length > 500)
                errors.Add("Nama Barang melebihi 500 karakter");
            if (!string.IsNullOrEmpty(data.Kd_UPB) && data.Kd_UPB.Length > 50)
                errors.Add("Kode UPB melebihi 50 karakter");
            if (!string.IsNullOrEmpty(data.Alamat) && data.Alamat.Length > 500)
                errors.Add("Alamat melebihi 500 karakter");
            if (!string.IsNullOrEmpty(data.No_Polisi) && data.No_Polisi.Length > 50)
                errors.Add("No Polisi melebihi 50 karakter");

            // 8. Validate Kapitalisasi - check parent asset exists
            if (!string.IsNullOrWhiteSpace(data.Jn_Kap) &&
                data.Jn_Kap.Contains("KAPITALISASI", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(data.Reg_Kap))
                {
                    errors.Add("Kapitalisasi harus memiliki Register Induk (Reg_Kap)");
                }
                else if (existingAssets.Any())
                {
                    // Check if parent asset exists by register
                    var parentAsset = existingAssets.FirstOrDefault(a =>
                        a.NomorRegister == data.Reg_Kap);

                    if (parentAsset == null)
                    {
                        errors.Add($"Aset induk dengan register '{data.Reg_Kap}' tidak ditemukan");
                    }
                }
            }

            // Set verification status
            if (errors.Count > 0)
            {
                data.VerificationStatus = "Invalid";
                data.ValidationErrors = string.Join("; ", errors);
                result.InvalidCount++;
            }
            else
            {
                data.VerificationStatus = "Valid";
                data.ValidationErrors = null;
                result.ValidCount++;
            }
        }

        await _context.SaveChangesAsync();

        // Update file status
        var importFile = await _context.ImportFiles.FindAsync(fileId);
        if (importFile != null)
        {
            importFile.VerifiedRows = result.ValidCount;
            importFile.ErrorRows = result.InvalidCount;
            importFile.Status = result.InvalidCount == 0 ? ImportStatus.Verified : ImportStatus.Imported;
            importFile.VerifiedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        result.TotalCount = importData.Count;
        return result;
    }

    /// <summary>
    /// Move verified data to actual Asset table
    /// </summary>
    public async Task<int> FinalizeImportAsync(int fileId, int skpdId)
    {
        var validData = await _context.AssetImportDatas
            .Where(a => a.ImportFileId == fileId && a.VerificationStatus == "Valid")
            .ToListAsync();

        int successCount = 0;

        foreach (var data in validData)
        {
            try
            {
                // Find or create KodeBarang
                var kodeBarang = await _context.KodeBarangs
                    .FirstOrDefaultAsync(k => k.KodeBarang1 == data.Kode_Barang);

                if (kodeBarang == null) continue;

                // Determine KIB type from Jns_Aset
                var kibType = DetermineKIBType(data.Jns_Aset);

                var asset = new Asset
                {
                    NomorRegister = data.Urut?.ToString("D5") ?? "00000",
                    KodeBarangId = kodeBarang.Id,
                    NamaBarang = data.Nm_Barang ?? "Tidak diketahui",
                    SkpdId = skpdId,
                    KodeRekening = ExtractKodeRekening(data.Kode_Barang),
                    Ruangan = data.Nm_Ruang,
                    Alamat = data.Alamat,
                    TanggalPerolehan = data.Tgl_Perolehan ?? DateTime.Now,
                    AsalUsul = data.Asal_Usul,
                    Harga = data.Harga ?? 0,
                    Keterangan = data.Keterangan,
                    Tahun = data.Tgl_Perolehan?.Year ?? DateTime.Now.Year,
                    KIBType = kibType,
                    Merk = data.Penggunaan_Merk,
                    Bahan = data.Bahan,
                    NomorRangka = data.No_Rangka,
                    NomorMesin = data.No_Mesin,
                    NomorPolisi = data.No_Polisi,
                    NomorBPKB = data.No_BPKB,
                    Panjang = data.Panjang,
                    Lebar = data.Lebar,
                    Luas = data.Luas,
                    Kondisi = data.Kon_b,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Assets.Add(asset);
                data.IsProcessed = true;
                successCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finalizing import for row {RowNumber}", data.RowNumber);
            }
        }

        await _context.SaveChangesAsync();

        // Update file status
        await UpdateStatusAsync(fileId, ImportStatus.Completed);

        return successCount;
    }

    private Data.Enums.KIBType DetermineKIBType(string? jnsAset)
    {
        if (string.IsNullOrWhiteSpace(jnsAset)) return Data.Enums.KIBType.B;

        return jnsAset.ToLower() switch
        {
            var s when s.Contains("tanah") => Data.Enums.KIBType.A,
            var s when s.Contains("peralatan") || s.Contains("mesin") => Data.Enums.KIBType.B,
            var s when s.Contains("gedung") || s.Contains("bangunan") => Data.Enums.KIBType.C,
            var s when s.Contains("jalan") || s.Contains("irigasi") || s.Contains("jaringan") => Data.Enums.KIBType.D,
            var s when s.Contains("tetap lainnya") => Data.Enums.KIBType.E,
            var s when s.Contains("konstruksi") => Data.Enums.KIBType.F,
            _ => Data.Enums.KIBType.B
        };
    }

    private string ExtractKodeRekening(string? kodeBarang)
    {
        if (string.IsNullOrWhiteSpace(kodeBarang)) return "0.0.0.00.00";

        // Extract first 5 segments as kode rekening
        var parts = kodeBarang.Split('.');
        if (parts.Length >= 5)
        {
            return string.Join(".", parts.Take(5));
        }
        return kodeBarang;
    }
}

/// <summary>
/// Verification result
/// </summary>
public class VerificationResult
{
    public int TotalCount { get; set; }
    public int ValidCount { get; set; }
    public int InvalidCount { get; set; }
}
