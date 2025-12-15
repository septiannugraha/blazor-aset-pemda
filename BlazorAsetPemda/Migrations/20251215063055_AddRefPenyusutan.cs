using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorAsetPemda.Migrations
{
    /// <inheritdoc />
    public partial class AddRefPenyusutan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RefPenyusutans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KodeBarang = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NamaBarang = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    KIBType = table.Column<int>(type: "int", nullable: true),
                    MasaManfaat = table.Column<int>(type: "int", nullable: true),
                    MasaManfaatBulan = table.Column<int>(type: "int", nullable: true),
                    PersenPenyusutan = table.Column<decimal>(type: "decimal(10,4)", nullable: true),
                    NilaiResidu = table.Column<decimal>(type: "decimal(10,4)", nullable: true),
                    MetodePenyusutan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Kelompok = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Keterangan = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TahunReferensi = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KodeBarangMasterId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefPenyusutans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefPenyusutans_KodeBarangs_KodeBarangMasterId",
                        column: x => x.KodeBarangMasterId,
                        principalTable: "KodeBarangs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefPenyusutans_KodeBarang",
                table: "RefPenyusutans",
                column: "KodeBarang");

            migrationBuilder.CreateIndex(
                name: "IX_RefPenyusutans_KodeBarangMasterId",
                table: "RefPenyusutans",
                column: "KodeBarangMasterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefPenyusutans");
        }
    }
}
