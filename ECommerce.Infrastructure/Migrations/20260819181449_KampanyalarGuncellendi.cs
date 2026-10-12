using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class KampanyalarGuncellendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KampanyaUrunler_Urunler_UrunId",
                table: "KampanyaUrunler");

            migrationBuilder.AlterColumn<Guid>(
                name: "UrunId",
                table: "KampanyaUrunler",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<DateTime>(
                name: "BaslangicTarihi",
                table: "KampanyaUrunler",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DinamikKurallarJson",
                table: "KampanyaUrunler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "IndirimDegeri",
                table: "KampanyaUrunler",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "IndirimTipi",
                table: "KampanyaUrunler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "KampanyaAciklamasi",
                table: "KampanyaUrunler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KampanyaAdi",
                table: "KampanyaUrunler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "KampanyaVaryasyonu",
                table: "KampanyaUrunler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "KapsamTuru",
                table: "KampanyaUrunler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "KategoriId",
                table: "KampanyaUrunler",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_KampanyaUrunler_Urunler_UrunId",
                table: "KampanyaUrunler",
                column: "UrunId",
                principalTable: "Urunler",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KampanyaUrunler_Urunler_UrunId",
                table: "KampanyaUrunler");

            migrationBuilder.DropColumn(
                name: "BaslangicTarihi",
                table: "KampanyaUrunler");

            migrationBuilder.DropColumn(
                name: "DinamikKurallarJson",
                table: "KampanyaUrunler");

            migrationBuilder.DropColumn(
                name: "IndirimDegeri",
                table: "KampanyaUrunler");

            migrationBuilder.DropColumn(
                name: "IndirimTipi",
                table: "KampanyaUrunler");

            migrationBuilder.DropColumn(
                name: "KampanyaAciklamasi",
                table: "KampanyaUrunler");

            migrationBuilder.DropColumn(
                name: "KampanyaAdi",
                table: "KampanyaUrunler");

            migrationBuilder.DropColumn(
                name: "KampanyaVaryasyonu",
                table: "KampanyaUrunler");

            migrationBuilder.DropColumn(
                name: "KapsamTuru",
                table: "KampanyaUrunler");

            migrationBuilder.DropColumn(
                name: "KategoriId",
                table: "KampanyaUrunler");

            migrationBuilder.AlterColumn<Guid>(
                name: "UrunId",
                table: "KampanyaUrunler",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_KampanyaUrunler_Urunler_UrunId",
                table: "KampanyaUrunler",
                column: "UrunId",
                principalTable: "Urunler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
