using System.ComponentModel.DataAnnotations;

namespace BlazorAsetPemda.Data.Models;

/// <summary>
/// Unit Pengguna Barang - Asset User Unit
/// </summary>
public class UPB
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string KodeUPB { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string NamaUPB { get; set; } = string.Empty;

    [Required]
    public int SkpdId { get; set; }
    public virtual SKPD SKPD { get; set; } = null!;

    [StringLength(255)]
    public string? PenanggungJawab { get; set; }

    [StringLength(50)]
    public string? NIP { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
}
