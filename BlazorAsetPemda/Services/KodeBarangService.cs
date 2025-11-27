using BlazorAsetPemda.Data;
using BlazorAsetPemda.Data.Models;
using BlazorAsetPemda.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace BlazorAsetPemda.Services;

public class KodeBarangService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public KodeBarangService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<(List<KodeBarang> KodeBarangs, int TotalCount)> GetKodeBarangsAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string? searchTerm = null,
        bool? isActive = null,
        KIBType? kibType = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var query = context.KodeBarangs.AsQueryable();

        // Search filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(k =>
                k.KodeBarang1.Contains(searchTerm) ||
                k.NamaBarang.Contains(searchTerm) ||
                (k.Satuan != null && k.Satuan.Contains(searchTerm)) ||
                (k.Spesifikasi != null && k.Spesifikasi.Contains(searchTerm)));
        }

        // Active status filter
        if (isActive.HasValue)
        {
            query = query.Where(k => k.IsActive == isActive.Value);
        }

        // KIB Type filter
        if (kibType.HasValue)
        {
            query = query.Where(k => k.KIBType == kibType.Value);
        }

        var totalCount = await query.CountAsync();

        var kodeBarangs = await query
            .OrderBy(k => k.KodeBarang1)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (kodeBarangs, totalCount);
    }

    public async Task<KodeBarang?> GetKodeBarangByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.KodeBarangs.FirstOrDefaultAsync(k => k.Id == id);
    }

    public async Task<bool> IsKodeBarangUniqueAsync(string kodeBarang, int? excludeId = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.KodeBarangs.Where(k => k.KodeBarang1 == kodeBarang);

        if (excludeId.HasValue)
        {
            query = query.Where(k => k.Id != excludeId.Value);
        }

        return !await query.AnyAsync();
    }

    public async Task<KodeBarang> CreateKodeBarangAsync(KodeBarang kodeBarang)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        kodeBarang.CreatedAt = DateTime.UtcNow;
        context.KodeBarangs.Add(kodeBarang);
        await context.SaveChangesAsync();

        return kodeBarang;
    }

    public async Task<KodeBarang> UpdateKodeBarangAsync(KodeBarang kodeBarang)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        kodeBarang.UpdatedAt = DateTime.UtcNow;
        context.KodeBarangs.Update(kodeBarang);
        await context.SaveChangesAsync();

        return kodeBarang;
    }

    public async Task<bool> DeleteKodeBarangAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var kodeBarang = await context.KodeBarangs.FindAsync(id);
        if (kodeBarang == null) return false;

        // Soft delete
        kodeBarang.IsActive = false;
        kodeBarang.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return true;
    }
}
