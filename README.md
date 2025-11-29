# Blazor Asset Management System (BMD - Barang Milik Daerah)

A comprehensive Blazor Server-based application for managing regional government assets (Barang Milik Daerah). The system supports multiple SKPD (Satuan Kerja Perangkat Daerah) with role-based access control, Excel import/export capabilities, depreciation calculations, and integration with SIMDA BMD database.

## Features

- **Role-Based Access Control**: Two main roles with separate layouts and menus
  - Pengguna Barang (Asset User)
  - Pengelola Barang (Asset Manager)
- **Multi-SKPD Support**: All regional departments can access the system
- **Asset Management**: Complete KIB A/B/C/D/E/F management
- **Depreciation Calculation**: Automated depreciation with interactive reporting
- **Excel Import/Export**: Import data from Excel, export to SIMDA BMD
- **Comprehensive Reporting**: Microsoft RDLC Report Viewer integration
- **Mobile-Responsive**: All menus compatible with mobile devices

## Technology Stack

- **Framework**: Blazor Server (.NET 8)
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: ASP.NET Core Identity with role-based authorization
- **Reporting**: Microsoft RDLC Report Viewer
- **UI**: Bootstrap 5 (mobile-responsive)
- **Excel**: EPPlus library

## Project Structure

```
BlazorAsetPemda/
├── Components/
│   ├── Layout/
│   │   ├── PenggunaBarangLayout.razor
│   │   ├── PengelolaBarangLayout.razor
│   │   ├── PenggunaBarangNavMenu.razor
│   │   └── PengelolaBarangNavMenu.razor
│   ├── Shared/
│   │   └── FilterComponent.razor
│   └── Pages/
│       ├── PenggunaBarang/
│       │   └── Dashboard.razor
│       └── PengelolaBarang/
│           └── Dashboard.razor
├── Data/
│   ├── ApplicationDbContext.cs
│   ├── ApplicationUser.cs
│   ├── SeedData.cs
│   ├── Enums/
│   │   ├── KIBType.cs
│   │   └── UserRole.cs
│   └── Models/
│       ├── SKPD.cs
│       ├── UPB.cs
│       ├── Asset.cs
│       ├── KodeBarang.cs
│       ├── KodeRekening.cs
│       ├── Kontrak.cs
│       ├── Depreciation.cs
│       └── KebijakanAkuntansi.cs
└── wwwroot/
```

## Getting Started

### Prerequisites

- .NET 8 SDK or later
- SQL Server 2019 or later (or SQL Server LocalDB)
- Visual Studio 2022 / VS Code / JetBrains Rider

### Installation

1. **Clone the repository** (if using git):
   ```bash
   cd d:\blazor_aset_pemda\BlazorAsetPemda
   ```

2. **Update the connection string** in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BlazorAsetPemdaDb;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
   }
   ```

   For production, use a proper SQL Server instance:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=BlazorAsetPemdaDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True"
     }
   }
   ```

3. **Create and run database migrations**:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

4. **Run the application**:
   ```bash
   dotnet run
   ```

5. **Access the application**:
   - Open your browser and navigate to `https://localhost:5001` or `http://localhost:5000`

### Default Login Credentials

The system comes with seeded demo accounts:

**Pengelola Barang (Asset Manager)**:
- Email: `pengelola@bmd.local`
- Password: `Pengelola123`

**Pengguna Barang (Asset User)**:
- Email: `pengguna@bmd.local`
- Password: `Pengguna123`

## User Roles & Menus

### Pengguna Barang (Asset User)

1. Dashboard
2. Data Umum
3. Saldo Awal (KIB A/B/C/D/E/F Lainnya)
4. Belanja Modal
   - Kontrak/Rincian Kontrak
   - Paket Pengguna
   - Jenis Belanja
   - Rincian Barang
   - BAST dan Distribusi
5. Inventarisasi, Pemanfaatan & Penghapusan
   - BA Cek Fisik
   - Rekapitulasi Hasil
   - Usulan Penghapusan
6. Pengamanan & Pemeliharaan
   - Sertifikat Tanah
   - IMB dan SLF
   - Kendaraan Dinas
7. Rekonsiliasi
   - BM-BHP
   - BM-Ekstra
   - Kapitalisasi BPBJ
   - Perhitungan Saldo Akhir
8. Penyusutan (KIB B/C/D/E/F Lainnya)
9. Laporan BMD

### Pengelola Barang (Asset Manager)

1. Dashboard
2. Data Umum
3. Saldo Awal (+ Monitoring)
4. Belanja Modal (+ Monitoring)
5. Status Pengguna
   - Penetapan Status
   - Pengembalian Status
   - Mutasi
6. Inventarisasi
   - Inventaris Awal
   - Inventarisasi Fisik
   - Rekapitulasi Hasil
   - Penentuan TL
   - Monitoring TL
7. Pengamanan & Pemeliharaan
8. Pemanfaatan
   - SK Pemanfaatan
   - BA Pemanfaatan
   - Monitoring
9. Penghapusan
   - Usulan Penghapusan
   - Penaksiran dan Penilaian
   - Penjualan
   - Pemusnahan
   - SK Penghapusan
10. Penyusutan (+ Monitoring)
11. **Admin Menu** (Pengelola only)
    - Pengguna Barang dan UPB
    - Kebijakan Akuntansi
    - Kode Barang
    - Kode Rekening

## Common UI Components

### Filter Component

Each page includes standardized filters:
- **Pengguna Barang**: Dropdown to select specific asset user/department
- **Jenis Tahun**:
  - "Sampai Dengan" (Up to year) - Shows cumulative data
  - "Hanya Tahun" (Only year) - Shows data for specific year only
- **Tahun**: Year selection (2010 - current year + 1)

## Database Models

### Core Entities

- **SKPD**: Regional government work units
- **UPB**: Asset user units
- **Asset**: Main asset entity with KIB classification
- **KodeBarang**: Asset codes/catalog
- **KodeRekening**: Account codes
- **Kontrak**: Procurement contracts
- **Depreciation**: Asset depreciation records
- **KebijakanAkuntansi**: Accounting policies for depreciation

### KIB Types

- **KIB A**: Tanah (Land)
- **KIB B**: Peralatan dan Mesin (Equipment and Machinery)
- **KIB C**: Gedung dan Bangunan (Buildings and Structures)
- **KIB D**: Jalan, Irigasi, dan Jaringan (Roads, Irrigation, and Networks)
- **KIB E**: Aset Tetap Lainnya (Other Fixed Assets)
- **KIB F**: Konstruksi Dalam Pengerjaan (Construction in Progress)
- **Lainnya**: Other

## Development

### Adding New Pages

1. Create a new Razor component in the appropriate folder:
   - `Components/Pages/PenggunaBarang/` for Pengguna Barang pages
   - `Components/Pages/PengelolaBarang/` for Pengelola Barang pages

2. Add the route and authorization:
   ```razor
   @page "/pengguna-barang/your-page"
   @attribute [Authorize(Roles = "PenggunaBarang")]
   @layout PenggunaBarangLayout
   ```

3. Include the FilterComponent if needed:
   ```razor
   <FilterComponent OnFilterChange="HandleFilterChange" />
   ```

### Database Migrations

After modifying models, create a new migration:

```bash
dotnet ef migrations add YourMigrationName
dotnet ef database update
```

### Running Tests

```bash
dotnet test
```

## Deployment

### Docker Deployment (Recommended)

Deploy with a single command using Docker Compose:

1. **Copy files to server**:

   ```bash
   scp -r . user@your-server:/path/to/app/
   ```

2. **Create environment file**:

   ```bash
   cp .env.example .env
   # Edit .env and set a strong password
   nano .env
   ```

3. **Start the application**:

   ```bash
   docker-compose up -d
   ```

4. **Access the application**:
   - URL: `http://your-server:5000`
   - Login: `pengelola@bmd.local` / `Pengelola123`

5. **View logs**:

   ```bash
   docker-compose logs -f blazor-app
   ```

6. **Stop the application**:

   ```bash
   docker-compose down
   ```

### Manual Deployment

#### Prerequisites for Production

1. Windows Server with IIS or Linux with Nginx/Apache
2. SQL Server 2019 or later
3. .NET 8 Runtime

#### Publish the Application

```bash
dotnet publish -c Release -o ./publish
```

#### IIS Deployment

1. Install the .NET 8 Hosting Bundle
2. Create an application pool with "No Managed Code"
3. Deploy the published files to IIS
4. Configure the connection string in `appsettings.json`

## Security Considerations

- Passwords are hashed using ASP.NET Core Identity
- Role-based authorization on all pages
- CSRF protection enabled
- SQL injection prevention via Entity Framework Core
- Secure HTTPS connections required in production

## Performance Optimization

- Server-side pagination for large datasets
- Lazy loading for navigation properties
- Asynchronous operations throughout
- Database indexes on frequently queried fields

## Contributing

This project is maintained by the development team. For questions or issues:

1. Check the documentation in [CLAUDE.md](CLAUDE.md)
2. Review existing issues
3. Create a detailed issue report with steps to reproduce

## License

Internal use only. All rights reserved.

## Support

For technical support, please contact the development team or refer to the documentation.

---

**Version**: 1.0.0
**Last Updated**: October 2025
**Framework**: .NET 8.0 / Blazor Server
