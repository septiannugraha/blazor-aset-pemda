using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlazorAsetPemda.Data.Enums;

namespace BlazorAsetPemda.Data.Models;

/// <summary>
/// RefPenyusutan - Reference table for depreciation parameters (masa manfaat, persentase)
/// Based on SIMDA BMD Ref_Penyusutan structure
/// </summary>
public class RefPenyusutan
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Kode Barang (e.g., "2.03.01.01.001")
    /// </summary>
    [Required]
    [StringLength(100)]
    public string KodeBarang { get; set; } = null!;

    /// <summary>
    /// Nama Barang
    /// </summary>
    [Required]
    [StringLength(500)]
    public string NamaBarang { get; set; } = null!;

    /// <summary>
    /// KIB Type (A/B/C/D/E/F)
    /// </summary>
    public KIBType? KIBType { get; set; }

    /// <summary>
    /// Masa Manfaat (tahun) - Useful life in years
    /// </summary>
    public int? MasaManfaat { get; set; }

    /// <summary>
    /// Masa Manfaat (bulan) - Useful life in months
    /// </summary>
    public int? MasaManfaatBulan { get; set; }

    /// <summary>
    /// Persentase Penyusutan per tahun (e.g., 25.00 for 25%)
    /// </summary>
    [Column(TypeName = "decimal(10,4)")]
    public decimal? PersenPenyusutan { get; set; }

    /// <summary>
    /// Nilai Residu / Residual value (usually 0 or percentage)
    /// </summary>
    [Column(TypeName = "decimal(10,4)")]
    public decimal? NilaiResidu { get; set; }

    /// <summary>
    /// Metode Penyusutan (Garis Lurus, dll)
    /// </summary>
    [StringLength(100)]
    public string? MetodePenyusutan { get; set; }

    /// <summary>
    /// Kelompok/Kategori
    /// </summary>
    [StringLength(200)]
    public string? Kelompok { get; set; }

    /// <summary>
    /// Keterangan tambahan
    /// </summary>
    [StringLength(500)]
    public string? Keterangan { get; set; }

    /// <summary>
    /// Tahun referensi (e.g., 2024)
    /// </summary>
    public int? TahunReferensi { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Optional: Foreign key to KodeBarang master
    public int? KodeBarangMasterId { get; set; }
    public virtual KodeBarang? KodeBarangMaster { get; set; }
}
