using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class siparissinifiGuncellendi2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AliciAdSoyad",
                table: "Siparisler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AliciTelefon",
                table: "Siparisler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FaturaTipi",
                table: "Siparisler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirmaAdi",
                table: "Siparisler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GonderenGorunenAd",
                table: "Siparisler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GonderimYeriAdi",
                table: "Siparisler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "KartNotu",
                table: "Siparisler",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OdemeYontemi",
                table: "Siparisler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TeslimatAdresi",
                table: "Siparisler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VergiNo",
                table: "Siparisler",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AliciAdSoyad",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "AliciTelefon",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "FaturaTipi",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "FirmaAdi",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "GonderenGorunenAd",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "GonderimYeriAdi",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "KartNotu",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "OdemeYontemi",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "TeslimatAdresi",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "VergiNo",
                table: "Siparisler");
        }
    }
}
