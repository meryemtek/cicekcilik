using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EntegrasyonAlanlariEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NetGsmBaslik",
                table: "SiteAyarlari",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NetGsmKullaniciAdi",
                table: "SiteAyarlari",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NetGsmSifre",
                table: "SiteAyarlari",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaytrMerchantId",
                table: "SiteAyarlari",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaytrMerchantKey",
                table: "SiteAyarlari",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaytrMerchantSalt",
                table: "SiteAyarlari",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NetGsmBaslik",
                table: "SiteAyarlari");

            migrationBuilder.DropColumn(
                name: "NetGsmKullaniciAdi",
                table: "SiteAyarlari");

            migrationBuilder.DropColumn(
                name: "NetGsmSifre",
                table: "SiteAyarlari");

            migrationBuilder.DropColumn(
                name: "PaytrMerchantId",
                table: "SiteAyarlari");

            migrationBuilder.DropColumn(
                name: "PaytrMerchantKey",
                table: "SiteAyarlari");

            migrationBuilder.DropColumn(
                name: "PaytrMerchantSalt",
                table: "SiteAyarlari");
        }
    }
}
