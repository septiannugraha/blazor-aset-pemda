using BlazorAsetPemda.Data;
using BlazorAsetPemda.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorAsetPemda.Services;

public class UPBService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public UPBService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<(List<UPB> UPBs, int TotalCount)> GetUPBsAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string? searchTerm = null,
        bool? isActive = null,
        int? skpdId = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var query = context.UPBs.Include(u => u.SKPD).AsQueryable();

        // Search filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(u =>
                u.KodeUPB.Contains(searchTerm) ||
                u.NamaUPB.Contains(searchTerm) ||
                (u.PenanggungJawab != null && u.PenanggungJawab.Contains(searchTerm)) ||
                (u.NIP != null && u.NIP.Contains(searchTerm)));
        }

        // Active status filter
        if (isActive.HasValue)
        {
            query = query.Where(u => u.IsActive == isActive.Value);
        }

        // SKPD filter
        if (skpdId.HasValue)
        {
            query = query.Where(u => u.SkpdId == skpdId.Value);
        }

        var totalCount = await query.CountAsync();

        var upbs = await query
            .OrderBy(u => u.KodeUPB)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (upbs, totalCount);
    }

    public async Task<UPB?> GetUPBByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.UPBs
            .Include(u => u.SKPD)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<bool> IsKodeUPBUniqueAsync(string kodeUPB, int? excludeId = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.UPBs.Where(u => u.KodeUPB == kodeUPB);

        if (excludeId.HasValue)
        {
            query = query.Where(u => u.Id != excludeId.Value);
        }

        return !await query.AnyAsync();
    }

    public async Task<UPB> CreateUPBAsync(UPB upb)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        upb.CreatedAt = DateTime.UtcNow;
        context.UPBs.Add(upb);
        await context.SaveChangesAsync();

        return upb;
    }

    public async Task<UPB> UpdateUPBAsync(UPB upb)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        upb.UpdatedAt = DateTime.UtcNow;
        context.UPBs.Update(upb);
        await context.SaveChangesAsync();

        return upb;
    }

    public async Task<bool> DeleteUPBAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var upb = await context.UPBs.FindAsync(id);
        if (upb == null) return false;

        // Soft delete
        upb.IsActive = false;
        upb.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<List<SKPD>> GetAllActiveSKPDsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.SKPDs
            .Where(s => s.IsActive)
            .OrderBy(s => s.KodeSKPD)
            .ToListAsync();
    }
}
