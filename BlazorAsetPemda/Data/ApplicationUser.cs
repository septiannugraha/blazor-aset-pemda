using Microsoft.AspNetCore.Identity;
using BlazorAsetPemda.Data.Models;

namespace BlazorAsetPemda.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public int? SkpdId { get; set; }
    public virtual SKPD? SKPD { get; set; }
    public int? UpbId { get; set; }
    public virtual UPB? UPB { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
}

