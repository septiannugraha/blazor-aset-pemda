using System.ComponentModel.DataAnnotations;

namespace BlazorAsetPemda.Data.Models;

/// <summary>
/// Kode Rekening - Account Code
/// </summary>
public class KodeRekening
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string KodeRekening1 { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string NamaRekening { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Deskripsi { get; set; }

    public int Level { get; set; }

    [StringLength(50)]
    public string? ParentKode { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
