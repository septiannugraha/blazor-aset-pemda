using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BlazorAsetPemda.Data.Models;
using BlazorAsetPemda.Data.Enums;

namespace BlazorAsetPemda.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Ensure database is created
        // Use EnsureCreated for SQLite (migrations are SQL Server specific)
        // Use MigrateAsync for SQL Server
        if (context.Database.IsSqlite())
        {
            await context.Database.EnsureCreatedAsync();
        }
        else
        {
            await context.Database.MigrateAsync();
        }

        // Seed roles
        await SeedRolesAsync(roleManager);

        // Seed SKPD and UPB
        await SeedSKPDandUPBAsync(context);

        // Seed default users
        await SeedUsersAsync(userManager, context);

        // Seed Kebijakan Akuntansi
        await SeedKebijakanAkuntansiAsync(context);

        // Seed Kode Rekening
        await SeedKodeRekeningAsync(context);

        // Seed Kode Barang
        await SeedKodeBarangAsync(context);

        // Seed Sample Assets
        await SeedSampleAssetsAsync(context);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "PenggunaBarang", "PengelolaBarang" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task SeedSKPDandUPBAsync(ApplicationDbContext context)
    {
        if (!await context.SKPDs.AnyAsync())
        {
            var skpds = new List<SKPD>
            {
                new SKPD
                {
                    KodeSKPD = "1.01.01",
                    NamaSKPD = "Dinas Pendidikan",
                    Alamat = "Jl. Pendidikan No. 1",
                    Telepon = "021-12345678"
                },
                new SKPD
                {
                    KodeSKPD = "1.02.01",
                    NamaSKPD = "Dinas Kesehatan",
                    Alamat = "Jl. Kesehatan No. 2",
                    Telepon = "021-87654321"
                },
                new SKPD
                {
                    KodeSKPD = "1.03.01",
                    NamaSKPD = "Dinas Pekerjaan Umum",
                    Alamat = "Jl. PU No. 3",
                    Telepon = "021-11223344"
                }
            };

            await context.SKPDs.AddRangeAsync(skpds);
            await context.SaveChangesAsync();

            // Add UPB for each SKPD
            var upbs = new List<UPB>
            {
                new UPB
                {
                    KodeUPB = "1.01.01.01",
                    NamaUPB = "Sekretariat Dinas Pendidikan",
                    SkpdId = skpds[0].Id,
                    PenanggungJawab = "Budi Santoso",
                    NIP = "196501011990031001"
                },
                new UPB
                {
                    KodeUPB = "1.02.01.01",
                    NamaUPB = "Sekretariat Dinas Kesehatan",
                    SkpdId = skpds[1].Id,
                    PenanggungJawab = "Siti Aminah",
                    NIP = "196701011990032001"
                },
                new UPB
                {
                    KodeUPB = "1.03.01.01",
                    NamaUPB = "Sekretariat Dinas PU",
                    SkpdId = skpds[2].Id,
                    PenanggungJawab = "Ahmad Yani",
                    NIP = "196801011990031002"
                }
            };

            await context.UPBs.AddRangeAsync(upbs);
            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        // Get first SKPD
        var firstSKPD = await context.SKPDs.FirstOrDefaultAsync();
        var firstUPB = await context.UPBs.FirstOrDefaultAsync();

        if (firstSKPD == null) return;

        // Seed Pengelola Barang
        var pengelolaEmail = "pengelola@bmd.local";
        if (await userManager.FindByEmailAsync(pengelolaEmail) == null)
        {
            var pengelola = new ApplicationUser
            {
                UserName = pengelolaEmail,
                Email = pengelolaEmail,
                EmailConfirmed = true,
                FullName = "Administrator Pengelola Barang",
                SkpdId = firstSKPD.Id,
                UpbId = firstUPB?.Id,
                IsActive = true
            };

            var result = await userManager.CreateAsync(pengelola, "Pengelola123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(pengelola, "PengelolaBarang");
            }
        }

        // Seed Pengguna Barang
        var penggunaEmail = "pengguna@bmd.local";
        if (await userManager.FindByEmailAsync(penggunaEmail) == null)
        {
            var pengguna = new ApplicationUser
            {
                UserName = penggunaEmail,
                Email = penggunaEmail,
                EmailConfirmed = true,
                FullName = "User Pengguna Barang",
                SkpdId = firstSKPD.Id,
                UpbId = firstUPB?.Id,
                IsActive = true
            };

            var result = await userManager.CreateAsync(pengguna, "Pengguna123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(pengguna, "PenggunaBarang");
            }
        }
    }

    private static async Task SeedKebijakanAkuntansiAsync(ApplicationDbContext context)
    {
        if (!await context.KebijakanAkuntansis.AnyAsync())
        {
            var kebijakans = new List<KebijakanAkuntansi>
            {
                new KebijakanAkuntansi
                {
                    KIBType = KIBType.B,
                    NamaKebijakan = "Penyusutan Peralatan dan Mesin",
                    MasaManfaat = 5,
                    NilaiResidu = 0,
                    MetodePenyusutan = "Garis Lurus",
                    BatasKapitalisasi = 300000,
                    Keterangan = "Kebijakan penyusutan untuk peralatan dan mesin dengan masa manfaat 5 tahun"
                },
                new KebijakanAkuntansi
                {
                    KIBType = KIBType.C,
                    NamaKebijakan = "Penyusutan Gedung dan Bangunan",
                    MasaManfaat = 20,
                    NilaiResidu = 0,
                    MetodePenyusutan = "Garis Lurus",
                    BatasKapitalisasi = 10000000,
                    Keterangan = "Kebijakan penyusutan untuk gedung dan bangunan dengan masa manfaat 20 tahun"
                },
                new KebijakanAkuntansi
                {
                    KIBType = KIBType.D,
                    NamaKebijakan = "Penyusutan Jalan, Irigasi dan Jaringan",
                    MasaManfaat = 10,
                    NilaiResidu = 0,
                    MetodePenyusutan = "Garis Lurus",
                    BatasKapitalisasi = 5000000,
                    Keterangan = "Kebijakan penyusutan untuk jalan, irigasi dan jaringan dengan masa manfaat 10 tahun"
                },
                new KebijakanAkuntansi
                {
                    KIBType = KIBType.E,
                    NamaKebijakan = "Penyusutan Aset Tetap Lainnya",
                    MasaManfaat = 4,
                    NilaiResidu = 0,
                    MetodePenyusutan = "Garis Lurus",
                    BatasKapitalisasi = 300000,
                    Keterangan = "Kebijakan penyusutan untuk aset tetap lainnya dengan masa manfaat 4 tahun"
                }
            };

            await context.KebijakanAkuntansis.AddRangeAsync(kebijakans);
            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedKodeRekeningAsync(ApplicationDbContext context)
    {
        if (!await context.KodeRekenings.AnyAsync())
        {
            var rekenings = new List<KodeRekening>
            {
                new KodeRekening
                {
                    KodeRekening1 = "1.3.1",
                    NamaRekening = "Aset Tetap",
                    Level = 1,
                    Deskripsi = "Aset Tetap"
                },
                new KodeRekening
                {
                    KodeRekening1 = "1.3.1.01",
                    NamaRekening = "Tanah",
                    Level = 2,
                    ParentKode = "1.3.1",
                    Deskripsi = "Aset Tetap berupa Tanah"
                },
                new KodeRekening
                {
                    KodeRekening1 = "1.3.1.02",
                    NamaRekening = "Peralatan dan Mesin",
                    Level = 2,
                    ParentKode = "1.3.1",
                    Deskripsi = "Aset Tetap berupa Peralatan dan Mesin"
                },
                new KodeRekening
                {
                    KodeRekening1 = "1.3.1.03",
                    NamaRekening = "Gedung dan Bangunan",
                    Level = 2,
                    ParentKode = "1.3.1",
                    Deskripsi = "Aset Tetap berupa Gedung dan Bangunan"
                },
                new KodeRekening
                {
                    KodeRekening1 = "1.3.1.04",
                    NamaRekening = "Jalan, Irigasi, dan Jaringan",
                    Level = 2,
                    ParentKode = "1.3.1",
                    Deskripsi = "Aset Tetap berupa Jalan, Irigasi, dan Jaringan"
                },
                new KodeRekening
                {
                    KodeRekening1 = "1.3.1.05",
                    NamaRekening = "Aset Tetap Lainnya",
                    Level = 2,
                    ParentKode = "1.3.1",
                    Deskripsi = "Aset Tetap Lainnya"
                },
                new KodeRekening
                {
                    KodeRekening1 = "1.3.1.06",
                    NamaRekening = "Konstruksi Dalam Pengerjaan",
                    Level = 2,
                    ParentKode = "1.3.1",
                    Deskripsi = "Aset Tetap berupa Konstruksi Dalam Pengerjaan"
                }
            };

            await context.KodeRekenings.AddRangeAsync(rekenings);
            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedKodeBarangAsync(ApplicationDbContext context)
    {
        if (!await context.KodeBarangs.AnyAsync())
        {
            var kodeBarangs = new List<KodeBarang>
            {
                new KodeBarang
                {
                    KodeBarang1 = "1.3.2.01.01",
                    NamaBarang = "Laptop",
                    KIBType = KIBType.B,
                    Satuan = "Unit",
                    MasaManfaat = 5,
                    NilaiResidu = 0,
                    Spesifikasi = "Laptop untuk kebutuhan kantor"
                },
                new KodeBarang
                {
                    KodeBarang1 = "1.3.2.01.02",
                    NamaBarang = "Kendaraan Roda 4",
                    KIBType = KIBType.B,
                    Satuan = "Unit",
                    MasaManfaat = 5,
                    NilaiResidu = 10,
                    Spesifikasi = "Mobil dinas operasional"
                },
                new KodeBarang
                {
                    KodeBarang1 = "1.3.2.01.03",
                    NamaBarang = "Motor Dinas",
                    KIBType = KIBType.B,
                    Satuan = "Unit",
                    MasaManfaat = 5,
                    NilaiResidu = 5,
                    Spesifikasi = "Sepeda motor untuk tugas lapangan"
                },
                new KodeBarang
                {
                    KodeBarang1 = "1.3.3.01.01",
                    NamaBarang = "Gedung Kantor",
                    KIBType = KIBType.C,
                    Satuan = "Unit",
                    MasaManfaat = 20,
                    NilaiResidu = 0,
                    Spesifikasi = "Gedung perkantoran permanen"
                },
                new KodeBarang
                {
                    KodeBarang1 = "1.3.2.01.04",
                    NamaBarang = "Meja Kerja",
                    KIBType = KIBType.B,
                    Satuan = "Unit",
                    MasaManfaat = 5,
                    NilaiResidu = 0,
                    Spesifikasi = "Meja kerja kayu"
                }
            };

            await context.KodeBarangs.AddRangeAsync(kodeBarangs);
            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedSampleAssetsAsync(ApplicationDbContext context)
    {
        if (!await context.Assets.AnyAsync())
        {
            var skpd = await context.SKPDs.FirstOrDefaultAsync();
            var upb = await context.UPBs.FirstOrDefaultAsync();
            var kodeBarangs = await context.KodeBarangs.ToListAsync();

            if (skpd == null || kodeBarangs.Count == 0) return;

            var assets = new List<Asset>
            {
                new Asset
                {
                    NomorRegister = "1.01.01-B-2024-0001",
                    KodeBarangId = kodeBarangs.First(k => k.NamaBarang == "Laptop").Id,
                    NamaBarang = "Laptop Asus Vivobook",
                    SkpdId = skpd.Id,
                    UpbId = upb?.Id,
                    KodeRekening = "1.3.2.01",
                    Ruangan = "Ruang TU",
                    TanggalPerolehan = new DateTime(2024, 1, 15),
                    AsalUsul = "Pembelian",
                    Harga = 12500000,
                    Tahun = 2024,
                    KIBType = KIBType.B,
                    Merk = "Asus Vivobook 14",
                    Kondisi = "Baik",
                    Keterangan = "Laptop untuk administrasi"
                },
                new Asset
                {
                    NomorRegister = "1.01.01-B-2024-0002",
                    KodeBarangId = kodeBarangs.First(k => k.NamaBarang == "Laptop").Id,
                    NamaBarang = "Laptop HP Pavilion",
                    SkpdId = skpd.Id,
                    UpbId = upb?.Id,
                    KodeRekening = "1.3.2.01",
                    Ruangan = "Ruang Kepala",
                    TanggalPerolehan = new DateTime(2024, 2, 10),
                    AsalUsul = "Pembelian",
                    Harga = 15000000,
                    Tahun = 2024,
                    KIBType = KIBType.B,
                    Merk = "HP Pavilion 15",
                    Kondisi = "Baik",
                    Keterangan = "Laptop untuk kepala dinas"
                },
                new Asset
                {
                    NomorRegister = "1.01.01-B-2023-0001",
                    KodeBarangId = kodeBarangs.First(k => k.NamaBarang == "Kendaraan Roda 4").Id,
                    NamaBarang = "Toyota Avanza",
                    SkpdId = skpd.Id,
                    UpbId = upb?.Id,
                    KodeRekening = "1.3.2.01",
                    TanggalPerolehan = new DateTime(2023, 5, 20),
                    AsalUsul = "Pembelian",
                    Harga = 235000000,
                    Tahun = 2023,
                    KIBType = KIBType.B,
                    Merk = "Toyota",
                    NomorRangka = "MHKA4BA1JLK012345",
                    NomorMesin = "1NRFE012345",
                    NomorPolisi = "B 1234 XYZ",
                    NomorBPKB = "1234567890AB",
                    Bahan = "Bensin",
                    Kondisi = "Baik",
                    Keterangan = "Mobil dinas operasional"
                },
                new Asset
                {
                    NomorRegister = "1.01.01-B-2023-0002",
                    KodeBarangId = kodeBarangs.First(k => k.NamaBarang == "Motor Dinas").Id,
                    NamaBarang = "Honda Vario 150",
                    SkpdId = skpd.Id,
                    UpbId = upb?.Id,
                    KodeRekening = "1.3.2.01",
                    TanggalPerolehan = new DateTime(2023, 8, 15),
                    AsalUsul = "Hibah",
                    Harga = 22000000,
                    Tahun = 2023,
                    KIBType = KIBType.B,
                    Merk = "Honda",
                    NomorRangka = "MH1KE1118LK678901",
                    NomorMesin = "KE11E2678901",
                    NomorPolisi = "B 5678 ABC",
                    NomorBPKB = "9876543210CD",
                    Bahan = "Bensin",
                    Kondisi = "Baik",
                    Keterangan = "Motor untuk tugas lapangan"
                },
                new Asset
                {
                    NomorRegister = "1.01.01-C-2022-0001",
                    KodeBarangId = kodeBarangs.First(k => k.NamaBarang == "Gedung Kantor").Id,
                    NamaBarang = "Gedung Kantor Utama",
                    SkpdId = skpd.Id,
                    UpbId = upb?.Id,
                    KodeRekening = "1.3.3.01",
                    Alamat = "Jl. Pendidikan No. 1",
                    TanggalPerolehan = new DateTime(2022, 3, 1),
                    AsalUsul = "Pembangunan",
                    Harga = 5500000000,
                    Tahun = 2022,
                    KIBType = KIBType.C,
                    Panjang = 30,
                    Lebar = 20,
                    Luas = 600,
                    Tinggi = 12,
                    Kondisi = "Baik",
                    Keterangan = "Gedung kantor 3 lantai"
                },
                new Asset
                {
                    NomorRegister = "1.01.01-B-2024-0003",
                    KodeBarangId = kodeBarangs.First(k => k.NamaBarang == "Meja Kerja").Id,
                    NamaBarang = "Meja Kerja Kayu Jati",
                    SkpdId = skpd.Id,
                    UpbId = upb?.Id,
                    KodeRekening = "1.3.2.01",
                    Ruangan = "Ruang Staff",
                    TanggalPerolehan = new DateTime(2024, 3, 10),
                    AsalUsul = "Pembelian",
                    Harga = 3500000,
                    Tahun = 2024,
                    KIBType = KIBType.B,
                    Bahan = "Kayu Jati",
                    Kondisi = "Baik",
                    Keterangan = "Meja kerja untuk staff administrasi"
                }
            };

            await context.Assets.AddRangeAsync(assets);
            await context.SaveChangesAsync();
        }
    }
}
