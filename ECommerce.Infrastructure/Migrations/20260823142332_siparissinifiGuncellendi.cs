using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class siparissinifiGuncellendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeslimatAdresi",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "TeslimatTarihi",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "TeslimatZamanAraligi",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "VipTeslimatMi",
                table: "Siparisler");

            migrationBuilder.AddColumn<decimal>(
                name: "AraToplam",
                table: "Siparisler",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "KampanyaIndirimTutari",
                table: "Siparisler",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "KullanilanKuponKodu",
                table: "Siparisler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "KuponIndirimTutari",
                table: "Siparisler",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TeslimatToplamUcreti",
                table: "Siparisler",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "UygulananKampanyaAdi",
                table: "Siparisler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VipToplamUcreti",
                table: "Siparisler",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "GorselUrl",
                table: "SiparisKalemleri",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Il",
                table: "SiparisKalemleri",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ilce",
                table: "SiparisKalemleri",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Mahalle",
                table: "SiparisKalemleri",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TeslimatGunu",
                table: "SiparisKalemleri",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TeslimatSaati",
                table: "SiparisKalemleri",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TeslimatUcreti",
                table: "SiparisKalemleri",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "VipTeslimatAktifMi",
                table: "SiparisKalemleri",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "VipUcreti",
                table: "SiparisKalemleri",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AraToplam",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "KampanyaIndirimTutari",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "KullanilanKuponKodu",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "KuponIndirimTutari",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "TeslimatToplamUcreti",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "UygulananKampanyaAdi",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "VipToplamUcreti",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "GorselUrl",
                table: "SiparisKalemleri");

            migrationBuilder.DropColumn(
                name: "Il",
                table: "SiparisKalemleri");

            migrationBuilder.DropColumn(
                name: "Ilce",
                table: "SiparisKalemleri");

            migrationBuilder.DropColumn(
                name: "Mahalle",
                table: "SiparisKalemleri");

            migrationBuilder.DropColumn(
                name: "TeslimatGunu",
                table: "SiparisKalemleri");

            migrationBuilder.DropColumn(
                name: "TeslimatSaati",
                table: "SiparisKalemleri");

            migrationBuilder.DropColumn(
                name: "TeslimatUcreti",
                table: "SiparisKalemleri");

            migrationBuilder.DropColumn(
                name: "VipTeslimatAktifMi",
                table: "SiparisKalemleri");

            migrationBuilder.DropColumn(
                name: "VipUcreti",
                table: "SiparisKalemleri");

            migrationBuilder.AddColumn<string>(
                name: "TeslimatAdresi",
                table: "Siparisler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "TeslimatTarihi",
                table: "Siparisler",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "TeslimatZamanAraligi",
                table: "Siparisler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "VipTeslimatMi",
                table: "Siparisler",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
