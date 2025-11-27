using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorAsetPemda.Migrations
{
    /// <inheritdoc />
    public partial class AddBMDReferenceAndAssetImportTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetImportDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Urut = table.Column<int>(type: "int", nullable: true),
                    Kd_UPB = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Nm_UPB = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    No_Ruang = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Nm_Ruang = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Jns_Aset = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Kode_Barang = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Nm_Barang = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Jn_Kap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    No_Kap = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Reg_Kap = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Tgl_Perolehan = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Keterangan = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Kd_Posisi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Pemilik = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Asal_Usul = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Kon_b = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Jl_Barang = table.Column<int>(type: "int", nullable: true),
                    Harga = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Harga_Total = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Masa_Manfaat = table.Column<int>(type: "int", nullable: true),
                    KDP = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Intra_Ekstra = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Alamat = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Panjang = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Lebar = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Luas = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Hak_Tanah = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Penggunaan_Merk = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Type_Panjang = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CC_Lebar = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Bahan = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Tgl_Dokumen = table.Column<DateTime>(type: "datetime2", nullable: true),
                    No_Dokumen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status_Tanah = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Bertingkat = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Beton = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    No_Rangka = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    No_Mesin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    No_Polisi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    No_BPKB = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Nm_File = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Proc_Id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImportBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ImportedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImportedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    ValidationErrors = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetImportDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KodeRekeningBMDs",
                columns: table => new
                {
                    KodeRekening = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Akun = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Kelompok = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Jenis = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Objek = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    RincianObjek = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    SubRincianObjek = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    SubSubRincianObjek = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Uraian = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    KategoriAset = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KodeRekeningBMDs", x => x.KodeRekening);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetImportDatas");

            migrationBuilder.DropTable(
                name: "KodeRekeningBMDs");
        }
    }
}
