using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HiCariAyarlariEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TemaRengiHex",
                table: "SiteAyarlari",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HiCariApiKey",
                table: "SiteAyarlari",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HiCariApiUrl",
                table: "SiteAyarlari",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HiCariEntegrasyonAktifMi",
                table: "SiteAyarlari",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HiCariApiKey",
                table: "SiteAyarlari");

            migrationBuilder.DropColumn(
                name: "HiCariApiUrl",
                table: "SiteAyarlari");

            migrationBuilder.DropColumn(
                name: "HiCariEntegrasyonAktifMi",
                table: "SiteAyarlari");

            migrationBuilder.AlterColumn<string>(
                name: "TemaRengiHex",
                table: "SiteAyarlari",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
