using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BlazorAsetPemda.Data.Models;

namespace BlazorAsetPemda.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<SKPD> SKPDs { get; set; }
    public DbSet<UPB> UPBs { get; set; }
    public DbSet<KodeBarang> KodeBarangs { get; set; }
    public DbSet<KodeRekening> KodeRekenings { get; set; }
    public DbSet<KodeRekeningBMD> KodeRekeningBMDs { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<AssetImportData> AssetImportDatas { get; set; }
    public DbSet<Kontrak> Kontraks { get; set; }
    public DbSet<Depreciation> Depreciations { get; set; }
    public DbSet<KebijakanAkuntansi> KebijakanAkuntansis { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure decimal precision for financial values
        modelBuilder.Entity<Asset>()
            .Property(a => a.Harga)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Kontrak>()
            .Property(k => k.NilaiKontrak)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Depreciation>()
            .Property(d => d.NilaiBuku)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Depreciation>()
            .Property(d => d.NilaiPenyusutan)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Depreciation>()
            .Property(d => d.AkumulasiPenyusutan)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Depreciation>()
            .Property(d => d.NilaiBukuAkhir)
            .HasPrecision(18, 2);

        modelBuilder.Entity<KebijakanAkuntansi>()
            .Property(k => k.NilaiResidu)
            .HasPrecision(5, 2);

        modelBuilder.Entity<KebijakanAkuntansi>()
            .Property(k => k.BatasKapitalisasi)
            .HasPrecision(18, 2);

        // Configure decimal precision for Asset dimensions
        modelBuilder.Entity<Asset>()
            .Property(a => a.Panjang)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Asset>()
            .Property(a => a.Lebar)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Asset>()
            .Property(a => a.Luas)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Asset>()
            .Property(a => a.Tinggi)
            .HasPrecision(18, 2);

        modelBuilder.Entity<KodeBarang>()
            .Property(k => k.NilaiResidu)
            .HasPrecision(5, 2);

        // Configure relationships
        modelBuilder.Entity<ApplicationUser>()
            .HasOne(u => u.SKPD)
            .WithMany(s => s.Users)
            .HasForeignKey(u => u.SkpdId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ApplicationUser>()
            .HasOne(u => u.UPB)
            .WithMany(u => u.Users)
            .HasForeignKey(u => u.UpbId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UPB>()
            .HasOne(u => u.SKPD)
            .WithMany(s => s.UPBs)
            .HasForeignKey(u => u.SkpdId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Asset>()
            .HasOne(a => a.SKPD)
            .WithMany(s => s.Assets)
            .HasForeignKey(a => a.SkpdId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Asset>()
            .HasOne(a => a.KodeBarang)
            .WithMany(k => k.Assets)
            .HasForeignKey(a => a.KodeBarangId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Asset>()
            .HasOne(a => a.Kontrak)
            .WithMany(k => k.Assets)
            .HasForeignKey(a => a.KontrakId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Depreciation>()
            .HasOne(d => d.Asset)
            .WithMany(a => a.Depreciations)
            .HasForeignKey(d => d.AssetId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        modelBuilder.Entity<Asset>()
            .HasIndex(a => a.NomorRegister);

        modelBuilder.Entity<Asset>()
            .HasIndex(a => a.Tahun);

        modelBuilder.Entity<SKPD>()
            .HasIndex(s => s.KodeSKPD)
            .IsUnique();

        modelBuilder.Entity<Kontrak>()
            .HasIndex(k => k.NomorKontrak);

        modelBuilder.Entity<Depreciation>()
            .HasIndex(d => new { d.AssetId, d.Tahun, d.Bulan });
    }
}
