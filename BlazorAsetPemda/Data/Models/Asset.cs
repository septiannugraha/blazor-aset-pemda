using System.ComponentModel.DataAnnotations;
using BlazorAsetPemda.Data.Enums;

namespace BlazorAsetPemda.Data.Models;

/// <summary>
/// Asset (Barang) - Main asset entity
/// </summary>
public class Asset
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string NomorRegister { get; set; } = string.Empty;

    [Required]
    public int KodeBarangId { get; set; }
    public virtual KodeBarang KodeBarang { get; set; } = null!;

    [Required]
    [StringLength(255)]
    public string NamaBarang { get; set; } = string.Empty;

    [Required]
    public int SkpdId { get; set; }
    public virtual SKPD SKPD { get; set; } = null!;

    public int? UpbId { get; set; }
    public virtual UPB? UPB { get; set; }

    [Required]
    [StringLength(20)]
    public string KodeRekening { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Ruangan { get; set; }

    [StringLength(500)]
    public string? Alamat { get; set; }

    public DateTime TanggalPerolehan { get; set; }

    [StringLength(500)]
    public string? AsalUsul { get; set; }

    public decimal Harga { get; set; }

    [StringLength(500)]
    public string? Keterangan { get; set; }

    public int Tahun { get; set; }

    public KIBType KIBType { get; set; }

    // For specific KIB types
    [StringLength(500)]
    public string? Merk { get; set; }

    [StringLength(100)]
    public string? Bahan { get; set; }

    [StringLength(255)]
    public string? NomorPabrik { get; set; }

    [StringLength(255)]
    public string? NomorRangka { get; set; }

    [StringLength(255)]
    public string? NomorMesin { get; set; }

    [StringLength(255)]
    public string? NomorPolisi { get; set; }

    [StringLength(255)]
    public string? NomorBPKB { get; set; }

    public decimal? Panjang { get; set; }
    public decimal? Lebar { get; set; }
    public decimal? Luas { get; set; }
    public decimal? Tinggi { get; set; }

    [StringLength(100)]
    public string? Kondisi { get; set; }

    public int? KontrakId { get; set; }
    public virtual Kontrak? Kontrak { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<Depreciation> Depreciations { get; set; } = new List<Depreciation>();
}
