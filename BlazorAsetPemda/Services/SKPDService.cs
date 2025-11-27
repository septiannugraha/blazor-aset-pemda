using BlazorAsetPemda.Data;
using BlazorAsetPemda.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorAsetPemda.Services;

public class SKPDService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public SKPDService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<(List<SKPD> SKPDs, int TotalCount)> GetSKPDsAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string? searchTerm = null,
        bool? isActive = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var query = context.SKPDs.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(s =>
                s.KodeSKPD.Contains(searchTerm) ||
                s.NamaSKPD.Contains(searchTerm) ||
                (s.Alamat != null && s.Alamat.Contains(searchTerm)));
        }

        if (isActive.HasValue)
        {
            query = query.Where(s => s.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync();

        var skpds = await query
            .OrderBy(s => s.KodeSKPD)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (skpds, totalCount);
    }

    public async Task<SKPD?> GetSKPDByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.SKPDs.FindAsync(id);
    }

    public async Task<SKPD> CreateSKPDAsync(SKPD skpd)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        skpd.CreatedAt = DateTime.Now;
        skpd.IsActive = true;

        context.SKPDs.Add(skpd);
        await context.SaveChangesAsync();

        return skpd;
    }

    public async Task UpdateSKPDAsync(SKPD skpd)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        skpd.UpdatedAt = DateTime.Now;

        context.SKPDs.Update(skpd);
        await context.SaveChangesAsync();
    }

    public async Task DeleteSKPDAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var skpd = await context.SKPDs.FindAsync(id);
        if (skpd != null)
        {
            // Soft delete
            skpd.IsActive = false;
            skpd.UpdatedAt = DateTime.Now;
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsKodeSKPDUniqueAsync(string kodeSKPD, int? excludeId = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var query = context.SKPDs.Where(s => s.KodeSKPD == kodeSKPD);

        if (excludeId.HasValue)
        {
            query = query.Where(s => s.Id != excludeId.Value);
        }

        return !await query.AnyAsync();
    }
}
