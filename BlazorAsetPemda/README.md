# Blazor Aset Pemda (BMD - Barang Milik Daerah)

A comprehensive Blazor Server application for managing regional government assets in Indonesia. This system supports multi-SKPD access with role-based authentication, complete asset lifecycle management, depreciation calculations, and integration capabilities with SIMDA BMD database.

## Features

### Implemented Modules

#### 1. Data Umum (Master Data Management)
Complete CRUD operations for master data:
- **SKPD Management** - Regional work units (Satuan Kerja Perangkat Daerah)
- **UPB Management** - Asset user units (Unit Pengguna Barang) with SKPD relationships
- **Kode Barang** - Asset codes with KIB categorization (A/B/C/D/E/F)
- **Kode Rekening** - Account codes with hierarchical structure

#### 2. Saldo Awal (Opening Balance)
Comprehensive asset management system:
- Asset listing with advanced filtering (SKPD, UPB, KIB Type, Year)
- Dynamic asset forms with 5 accordion sections
- KIB-specific fields that show/hide based on asset type
- Auto-generate registration numbers
- Detailed asset view with organized information cards

### Planned Modules

The application includes placeholder pages for:
- **Belanja Modal** - Capital expenditure tracking
- **Status Pengguna** - User status management
- **Inventarisasi** - Physical inventory management
- **Pengamanan & Pemeliharaan** - Asset security and maintenance
- **Pemanfaatan** - Asset utilization tracking
- **Penghapusan** - Asset disposal management
- **Penyusutan** - Depreciation calculations
- **Admin** - System administration

## Technology Stack

- **Framework**: Blazor Server (.NET 8+)
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: ASP.NET Core Identity with role-based authorization
- **UI Framework**: Bootstrap 5 (responsive design)
- **Icons**: Bootstrap Icons
- **Architecture**: Service layer pattern with IDbContextFactory

## Key Technical Features

- **Thread-Safe Database Access** - IDbContextFactory pattern for Blazor Server
- **Role-Based Access Control** - Two main roles: Pengelola Barang & Pengguna Barang
- **Soft Delete Pattern** - IsActive flag for data preservation
- **Server-Side Pagination** - Configurable page sizes for large datasets
- **Cascading Dropdowns** - SKPD → UPB, KIB Type → Kode Barang
- **Dynamic Forms** - Fields adapt based on KIB asset type selection
- **Responsive Design** - Mobile-compatible UI
- **Scoped CSS** - Component-specific styling

## KIB Asset Types

The system supports Indonesian government asset classification:
- **KIB A** - Tanah (Land)
- **KIB B** - Peralatan dan Mesin (Equipment and Machinery)
- **KIB C** - Gedung dan Bangunan (Buildings)
- **KIB D** - Jalan, Irigasi, dan Jaringan (Roads, Irrigation, Networks)
- **KIB E** - Aset Tetap Lainnya (Other Fixed Assets)
- **KIB F** - Konstruksi Dalam Pengerjaan (Construction in Progress)

## Getting Started

### Prerequisites

- .NET 8 SDK or later
- SQL Server 2019+ or SQL Server LocalDB
- Visual Studio 2022, VS Code, or JetBrains Rider

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd BlazorAsetPemda
   ```

2. **Update database connection string**

   Edit `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BlazorAsetPemda;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
   }
   ```

3. **Apply database migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run --urls "http://localhost:5000;https://localhost:5001"
   ```

5. **Access the application**
   - HTTP: http://localhost:5000
   - HTTPS: https://localhost:5001

### Default Users

The system seeds two default users for testing:

**Pengelola Barang (Asset Manager)**
- Email: `pengelola@bmd.go.id`
- Password: `Pengelola123!`
- Role: Full system access with monitoring and administrative capabilities

**Pengguna Barang (Asset User)**
- Email: `pengguna@bmd.go.id`
- Password: `Pengguna123!`
- Role: Asset user with operational capabilities

## Project Structure

```
BlazorAsetPemda/
├── Components/
│   ├── Account/              # Identity/authentication pages
│   ├── Layout/               # Layout components
│   │   ├── PengelolaBarangLayout.razor
│   │   ├── PengelolaBarangNavMenu.razor
│   │   ├── PenggunaBarangLayout.razor
│   │   └── PenggunaBarangNavMenu.razor
│   ├── Pages/
│   │   ├── PengelolaBarang/  # Asset manager pages
│   │   │   ├── Dashboard.razor
│   │   │   ├── DataUmum/     # Master data (SKPD, UPB, codes)
│   │   │   ├── SaldoAwal/    # Opening balance/assets
│   │   │   ├── BelanjaModal/ # Capital expenditure
│   │   │   ├── Inventarisasi/
│   │   │   ├── Pengamanan/
│   │   │   ├── Pemanfaatan/
│   │   │   ├── Penghapusan/
│   │   │   ├── Penyusutan/
│   │   │   └── Admin/
│   │   └── PenggunaBarang/   # Asset user pages
│   └── Shared/               # Shared components
├── Data/
│   ├── ApplicationDbContext.cs
│   ├── ApplicationUser.cs
│   ├── Enums/                # KIBType, UserRole
│   └── Models/               # Entity models
├── Migrations/               # EF Core migrations
├── Services/                 # Business logic layer
│   ├── AssetService.cs
│   ├── SKPDService.cs
│   ├── UPBService.cs
│   ├── KodeBarangService.cs
│   └── KodeRekeningService.cs
└── wwwroot/                  # Static files
```

## Database Schema

### Main Entities

- **ApplicationUser** - Users with roles and SKPD/UPB assignments
- **SKPD** - Regional work units
- **UPB** - Asset user units
- **Asset** - Main asset records with KIB classification
- **KodeBarang** - Asset classification codes
- **KodeRekening** - Account codes
- **Kontrak** - Contract information
- **KebijakanAkuntansi** - Accounting policies
- **Depreciation** - Depreciation records

All entities include:
- `IsActive` flag for soft deletes
- `CreatedAt` and `UpdatedAt` timestamps

## Development Guidelines

### Adding a New Page

1. Create the Razor component in appropriate folder
2. Add `@rendermode InteractiveServer` for interactivity
3. Add `@attribute [Authorize(Roles = "RoleName")]` for authorization
4. Set the appropriate layout: `@layout PengelolaBarangLayout` or `@layout PenggunaBarangLayout`
5. Update the corresponding navigation menu

### Creating a New Service

1. Create service class in `Services/` folder
2. Use `IDbContextFactory<ApplicationDbContext>` for thread safety
3. Implement async methods with proper error handling
4. Register in `Program.cs` with `builder.Services.AddScoped<YourService>()`

### Styling Components

- Use Bootstrap 5 classes for layout and components
- Create scoped CSS files (`.razor.css`) for component-specific styles
- Use Bootstrap Icons for icons (`<i class="bi bi-icon-name"></i>`)

## Security

- **Authentication** - ASP.NET Core Identity
- **Authorization** - Role-based with `[Authorize]` attributes
- **CSRF Protection** - Enabled by default
- **SQL Injection Prevention** - EF Core parameterized queries
- **Password Hashing** - Secure password storage

## Performance Optimization

- IDbContextFactory for thread-safe, efficient database access
- Server-side pagination for large datasets
- Lazy loading for related entities
- Async/await throughout the application

## Future Enhancements

- Excel import/export functionality
- SIMDA BMD database integration
- RDLC report generation
- Advanced depreciation calculations
- Physical inventory mobile app
- Barcode/QR code integration
- Document management system
- Audit trail logging

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is proprietary software for regional government asset management.

## Support

For technical support or questions, please contact the development team.

---

**Version**: 1.0.0
**Last Updated**: October 2024
**Framework**: .NET 8 / Blazor Server
