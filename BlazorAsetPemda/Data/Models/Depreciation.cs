using System.ComponentModel.DataAnnotations;

namespace BlazorAsetPemda.Data.Models;

/// <summary>
/// Depreciation (Penyusutan) - Asset depreciation records
/// </summary>
public class Depreciation
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int AssetId { get; set; }
    public virtual Asset Asset { get; set; } = null!;

    [Required]
    public int Tahun { get; set; }

    public int Bulan { get; set; }

    public decimal NilaiBuku { get; set; }

    public decimal NilaiPenyusutan { get; set; }

    public decimal AkumulasiPenyusutan { get; set; }

    public decimal NilaiBukuAkhir { get; set; }

    public int MasaManfaatSisa { get; set; } // dalam bulan

    [StringLength(500)]
    public string? Keterangan { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
