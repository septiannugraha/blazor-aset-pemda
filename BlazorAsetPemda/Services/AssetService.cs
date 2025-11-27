using Microsoft.EntityFrameworkCore;
using BlazorAsetPemda.Data;
using BlazorAsetPemda.Data.Models;
using BlazorAsetPemda.Data.Enums;

namespace BlazorAsetPemda.Services;

public class AssetService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public AssetService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<(List<Asset> Assets, int TotalCount)> GetAssetsAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string? searchTerm = null,
        int? skpdId = null,
        int? upbId = null,
        KIBType? kibType = null,
        int? tahun = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var query = context.Assets
            .Include(a => a.SKPD)
            .Include(a => a.UPB)
            .Include(a => a.KodeBarang)
            .Include(a => a.Kontrak)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(a =>
                a.NamaBarang.Contains(searchTerm) ||
                a.NomorRegister.Contains(searchTerm) ||
                (a.Keterangan != null && a.Keterangan.Contains(searchTerm)));
        }

        if (skpdId.HasValue)
        {
            query = query.Where(a => a.SkpdId == skpdId.Value);
        }

        if (upbId.HasValue)
        {
            query = query.Where(a => a.UpbId == upbId.Value);
        }

        if (kibType.HasValue)
        {
            query = query.Where(a => a.KIBType == kibType.Value);
        }

        if (tahun.HasValue)
        {
            query = query.Where(a => a.Tahun == tahun.Value);
        }

        query = query.Where(a => a.IsActive);

        var totalCount = await query.CountAsync();

        var assets = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (assets, totalCount);
    }

    public async Task<Asset?> GetAssetByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Assets
            .Include(a => a.SKPD)
            .Include(a => a.UPB)
            .Include(a => a.KodeBarang)
            .Include(a => a.Kontrak)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Asset> CreateAssetAsync(Asset asset)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        asset.CreatedAt = DateTime.UtcNow;
        asset.IsActive = true;

        context.Assets.Add(asset);
        await context.SaveChangesAsync();

        return asset;
    }

    public async Task<Asset> UpdateAssetAsync(Asset asset)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        asset.UpdatedAt = DateTime.UtcNow;

        context.Assets.Update(asset);
        await context.SaveChangesAsync();

        return asset;
    }

    public async Task<bool> DeleteAssetAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var asset = await context.Assets.FindAsync(id);
        if (asset == null)
            return false;

        // Soft delete
        asset.IsActive = false;
        asset.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<List<SKPD>> GetAllSKPDsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.SKPDs
            .Where(s => s.IsActive)
            .OrderBy(s => s.NamaSKPD)
            .ToListAsync();
    }

    public async Task<List<UPB>> GetUPBsBySKPDAsync(int skpdId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.UPBs
            .Where(u => u.SkpdId == skpdId && u.IsActive)
            .OrderBy(u => u.NamaUPB)
            .ToListAsync();
    }

    public async Task<List<KodeBarang>> GetKodeBarangsByKIBAsync(KIBType kibType)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.KodeBarangs
            .Where(k => k.KIBType == kibType && k.IsActive)
            .OrderBy(k => k.NamaBarang)
            .ToListAsync();
    }

    public async Task<List<KodeBarang>> GetAllActiveKodeBarangsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.KodeBarangs
            .Where(k => k.IsActive)
            .OrderBy(k => k.KodeBarang1)
            .ToListAsync();
    }

    public async Task<List<Kontrak>> GetActiveKontraksAsync(int? skpdId = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.Kontraks.AsQueryable();

        if (skpdId.HasValue)
        {
            query = query.Where(k => k.SkpdId == skpdId.Value);
        }

        return await query
            .Where(k => k.IsActive)
            .OrderByDescending(k => k.TanggalKontrak)
            .ToListAsync();
    }

    public async Task<string> GenerateNomorRegisterAsync(int skpdId, KIBType kibType, int tahun)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var skpd = await context.SKPDs.FindAsync(skpdId);
        if (skpd == null)
            return string.Empty;

        // Get the last register number for this SKPD, KIB, and year
        var lastAsset = await context.Assets
            .Where(a => a.SkpdId == skpdId && a.KIBType == kibType && a.Tahun == tahun)
            .OrderByDescending(a => a.NomorRegister)
            .FirstOrDefaultAsync();

        int nextNumber = 1;
        if (lastAsset != null)
        {
            // Extract the number from the last register (assuming format: SKPD-KIB-YYYY-NNNN)
            var parts = lastAsset.NomorRegister.Split('-');
            if (parts.Length >= 4 && int.TryParse(parts[3], out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        // Format: KODESKPD-KIB-YYYY-NNNN
        return $"{skpd.KodeSKPD}-{kibType}-{tahun}-{nextNumber:D4}";
    }
}
