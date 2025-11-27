using BlazorAsetPemda.Data;
using BlazorAsetPemda.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorAsetPemda.Services;

public class KodeRekeningService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public KodeRekeningService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<(List<KodeRekening> KodeRekenings, int TotalCount)> GetKodeRekeningsAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string? searchTerm = null,
        bool? isActive = null,
        int? level = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var query = context.KodeRekenings.AsQueryable();

        // Search filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(k =>
                k.KodeRekening1.Contains(searchTerm) ||
                k.NamaRekening.Contains(searchTerm) ||
                (k.Deskripsi != null && k.Deskripsi.Contains(searchTerm)));
        }

        // Active status filter
        if (isActive.HasValue)
        {
            query = query.Where(k => k.IsActive == isActive.Value);
        }

        // Level filter
        if (level.HasValue)
        {
            query = query.Where(k => k.Level == level.Value);
        }

        var totalCount = await query.CountAsync();

        var kodeRekenings = await query
            .OrderBy(k => k.KodeRekening1)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (kodeRekenings, totalCount);
    }

    public async Task<KodeRekening?> GetKodeRekeningByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.KodeRekenings.FirstOrDefaultAsync(k => k.Id == id);
    }

    public async Task<bool> IsKodeRekeningUniqueAsync(string kodeRekening, int? excludeId = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.KodeRekenings.Where(k => k.KodeRekening1 == kodeRekening);

        if (excludeId.HasValue)
        {
            query = query.Where(k => k.Id != excludeId.Value);
        }

        return !await query.AnyAsync();
    }

    public async Task<KodeRekening> CreateKodeRekeningAsync(KodeRekening kodeRekening)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        kodeRekening.CreatedAt = DateTime.UtcNow;
        context.KodeRekenings.Add(kodeRekening);
        await context.SaveChangesAsync();

        return kodeRekening;
    }

    public async Task<KodeRekening> UpdateKodeRekeningAsync(KodeRekening kodeRekening)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        kodeRekening.UpdatedAt = DateTime.UtcNow;
        context.KodeRekenings.Update(kodeRekening);
        await context.SaveChangesAsync();

        return kodeRekening;
    }

    public async Task<bool> DeleteKodeRekeningAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var kodeRekening = await context.KodeRekenings.FindAsync(id);
        if (kodeRekening == null) return false;

        // Soft delete
        kodeRekening.IsActive = false;
        kodeRekening.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<List<KodeRekening>> GetAllActiveKodeRekeningsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.KodeRekenings
            .Where(k => k.IsActive)
            .OrderBy(k => k.KodeRekening1)
            .ToListAsync();
    }
}
