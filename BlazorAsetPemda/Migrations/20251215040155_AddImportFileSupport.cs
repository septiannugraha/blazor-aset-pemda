using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorAsetPemda.Migrations
{
    /// <inheritdoc />
    public partial class AddImportFileSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImportFileId",
                table: "AssetImportDatas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RowNumber",
                table: "AssetImportDatas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationStatus",
                table: "AssetImportDatas",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ImportFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    StoredFileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    ImportType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TotalRows = table.Column<int>(type: "int", nullable: false),
                    ImportedRows = table.Column<int>(type: "int", nullable: false),
                    ErrorRows = table.Column<int>(type: "int", nullable: false),
                    VerifiedRows = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ImportedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportFiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetImportDatas_ImportFileId",
                table: "AssetImportDatas",
                column: "ImportFileId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetImportDatas_VerificationStatus",
                table: "AssetImportDatas",
                column: "VerificationStatus");

            migrationBuilder.CreateIndex(
                name: "IX_ImportFiles_BatchId",
                table: "ImportFiles",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportFiles_ImportType",
                table: "ImportFiles",
                column: "ImportType");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetImportDatas_ImportFiles_ImportFileId",
                table: "AssetImportDatas",
                column: "ImportFileId",
                principalTable: "ImportFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetImportDatas_ImportFiles_ImportFileId",
                table: "AssetImportDatas");

            migrationBuilder.DropTable(
                name: "ImportFiles");

            migrationBuilder.DropIndex(
                name: "IX_AssetImportDatas_ImportFileId",
                table: "AssetImportDatas");

            migrationBuilder.DropIndex(
                name: "IX_AssetImportDatas_VerificationStatus",
                table: "AssetImportDatas");

            migrationBuilder.DropColumn(
                name: "ImportFileId",
                table: "AssetImportDatas");

            migrationBuilder.DropColumn(
                name: "RowNumber",
                table: "AssetImportDatas");

            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                table: "AssetImportDatas");
        }
    }
}
