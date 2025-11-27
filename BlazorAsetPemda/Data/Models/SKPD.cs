using System.ComponentModel.DataAnnotations;

namespace BlazorAsetPemda.Data.Models;

/// <summary>
/// Satuan Kerja Perangkat Daerah (SKPD) - Regional Government Work Unit
/// </summary>
public class SKPD
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string KodeSKPD { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string NamaSKPD { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Alamat { get; set; }

    [StringLength(50)]
    public string? Telepon { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<UPB> UPBs { get; set; } = new List<UPB>();
    public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    public virtual ICollection<Asset> Assets { get; set; } = new List<Asset>();
}
