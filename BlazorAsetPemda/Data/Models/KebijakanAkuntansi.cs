using System.ComponentModel.DataAnnotations;
using BlazorAsetPemda.Data.Enums;

namespace BlazorAsetPemda.Data.Models;

/// <summary>
/// Kebijakan Akuntansi - Accounting Policy for asset depreciation
/// </summary>
public class KebijakanAkuntansi
{
    [Key]
    public int Id { get; set; }

    [Required]
    public KIBType KIBType { get; set; }

    [Required]
    [StringLength(255)]
    public string NamaKebijakan { get; set; } = string.Empty;

    [Required]
    public int MasaManfaat { get; set; } // dalam tahun

    public decimal NilaiResidu { get; set; } // dalam persen (0-100)

    [StringLength(50)]
    public string MetodePenyusutan { get; set; } = "Garis Lurus"; // Garis Lurus, Saldo Menurun, dll

    public decimal? BatasKapitalisasi { get; set; } // Nilai minimum untuk dikapitalisasi

    [StringLength(1000)]
    public string? Keterangan { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
