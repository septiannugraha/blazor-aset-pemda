using System.ComponentModel.DataAnnotations;
using BlazorAsetPemda.Data.Enums;

namespace BlazorAsetPemda.Data.Models;

/// <summary>
/// Kode Barang - Asset Code
/// </summary>
public class KodeBarang
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string KodeBarang1 { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string NamaBarang { get; set; } = string.Empty;

    [Required]
    public KIBType KIBType { get; set; }

    [StringLength(100)]
    public string? Satuan { get; set; }

    public int? MasaManfaat { get; set; } // Masa manfaat dalam tahun

    public decimal? NilaiResidu { get; set; } // Nilai residu dalam persen

    [StringLength(500)]
    public string? Spesifikasi { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<Asset> Assets { get; set; } = new List<Asset>();
}
