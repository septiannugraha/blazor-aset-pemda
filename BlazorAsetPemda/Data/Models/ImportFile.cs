using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorAsetPemda.Data.Models;

/// <summary>
/// Import file metadata - tracks all uploaded files for import
/// </summary>
public class ImportFile
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Unique batch ID for this import
    /// </summary>
    public Guid BatchId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Original file name
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Stored file name (with path)
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string StoredFileName { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Type of import: Aset, RealisasiBelanja, Kontrak
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string ImportType { get; set; } = string.Empty;

    /// <summary>
    /// Upload timestamp
    /// </summary>
    public DateTime UploadedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// User who uploaded the file
    /// </summary>
    [MaxLength(255)]
    public string? UploadedBy { get; set; }

    /// <summary>
    /// Import status: Pending, Imported, Verified, Completed, Failed
    /// </summary>
    [MaxLength(50)]
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// Number of rows in the file
    /// </summary>
    public int TotalRows { get; set; }

    /// <summary>
    /// Number of rows successfully imported
    /// </summary>
    public int ImportedRows { get; set; }

    /// <summary>
    /// Number of rows with errors
    /// </summary>
    public int ErrorRows { get; set; }

    /// <summary>
    /// Number of rows verified successfully
    /// </summary>
    public int VerifiedRows { get; set; }

    /// <summary>
    /// Notes/remarks
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Last import attempt timestamp
    /// </summary>
    public DateTime? ImportedAt { get; set; }

    /// <summary>
    /// Last verification timestamp
    /// </summary>
    public DateTime? VerifiedAt { get; set; }

    /// <summary>
    /// Final completion timestamp (moved to actual tables)
    /// </summary>
    public DateTime? CompletedAt { get; set; }
}

/// <summary>
/// Enum-like class for import types
/// </summary>
public static class ImportTypes
{
    public const string Aset = "Aset";
    public const string RealisasiBelanja = "RealisasiBelanja";
    public const string Kontrak = "Kontrak";
}

/// <summary>
/// Enum-like class for import statuses
/// </summary>
public static class ImportStatus
{
    public const string Pending = "Pending";
    public const string Imported = "Imported";
    public const string Verified = "Verified";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
}
