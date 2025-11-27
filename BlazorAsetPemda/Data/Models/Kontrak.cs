using System.ComponentModel.DataAnnotations;

namespace BlazorAsetPemda.Data.Models;

/// <summary>
/// Kontrak - Contract for asset procurement
/// </summary>
public class Kontrak
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string NomorKontrak { get; set; } = string.Empty;

    [Required]
    public DateTime TanggalKontrak { get; set; }

    [Required]
    [StringLength(500)]
    public string NamaPaket { get; set; } = string.Empty;

    [Required]
    public decimal NilaiKontrak { get; set; }

    [Required]
    [StringLength(500)]
    public string Penyedia { get; set; } = string.Empty;

    [StringLength(255)]
    public string? NPWP { get; set; }

    [StringLength(500)]
    public string? AlamatPenyedia { get; set; }

    public DateTime? TanggalMulai { get; set; }
    public DateTime? TanggalSelesai { get; set; }

    public int Tahun { get; set; }

    [Required]
    public int SkpdId { get; set; }
    public virtual SKPD SKPD { get; set; } = null!;

    [StringLength(100)]
    public string? JenisBelanja { get; set; }

    [StringLength(1000)]
    public string? Keterangan { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<Asset> Assets { get; set; } = new List<Asset>();
}
