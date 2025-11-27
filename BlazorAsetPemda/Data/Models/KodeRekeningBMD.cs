using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorAsetPemda.Data.Models;

/// <summary>
/// Reference table for BMD account codes based on Permendagri 108 Tahun 2016
/// Format: xx.xx.xx.xx.xx.xx.xx (Akun.Kelompok.Jenis.Objek.RincianObjek.SubRincianObjek.SubSubRincianObjek)
/// </summary>
public class KodeRekeningBMD
{
    /// <summary>
    /// Primary Key: Full account code in format xx.xx.xx.xx.xx.xx.xx
    /// Example: 1.3.1.1.1.1.1
    /// </summary>
    [Key]
    [MaxLength(50)]
    public string KodeRekening { get; set; } = string.Empty;

    /// <summary>
    /// Akun (Account) - Level 1
    /// Example: 1 for Aset Tetap
    /// </summary>
    [MaxLength(10)]
    public string? Akun { get; set; }

    /// <summary>
    /// Kelompok (Group) - Level 2
    /// Example: 3 for Aset Tetap category
    /// </summary>
    [MaxLength(10)]
    public string? Kelompok { get; set; }

    /// <summary>
    /// Jenis (Type) - Level 3
    /// Example: 1 for Tanah, 2 for Peralatan dan Mesin, etc.
    /// </summary>
    [MaxLength(10)]
    public string? Jenis { get; set; }

    /// <summary>
    /// Objek (Object) - Level 4
    /// </summary>
    [MaxLength(10)]
    public string? Objek { get; set; }

    /// <summary>
    /// Rincian Objek (Object Detail) - Level 5
    /// </summary>
    [MaxLength(10)]
    public string? RincianObjek { get; set; }

    /// <summary>
    /// Sub Rincian Objek (Sub Object Detail) - Level 6
    /// </summary>
    [MaxLength(10)]
    public string? SubRincianObjek { get; set; }

    /// <summary>
    /// Sub-Sub Rincian Objek (Sub-Sub Object Detail) - Level 7
    /// </summary>
    [MaxLength(10)]
    public string? SubSubRincianObjek { get; set; }

    /// <summary>
    /// Uraian (Description) - Full description of the account code
    /// Example: "TANAH BANGUNAN PERUMAHAN/G.TEMPAT TINGGAL"
    /// </summary>
    [MaxLength(500)]
    public string Uraian { get; set; } = string.Empty;

    /// <summary>
    /// Kategori aset (Tanah, Peralatan dan Mesin, Gedung dan Bangunan, etc.)
    /// Derived from Jenis level
    /// </summary>
    [MaxLength(100)]
    public string? KategoriAset { get; set; }

    /// <summary>
    /// Created timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last updated timestamp
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Is active record
    /// </summary>
    public bool IsActive { get; set; } = true;
}
