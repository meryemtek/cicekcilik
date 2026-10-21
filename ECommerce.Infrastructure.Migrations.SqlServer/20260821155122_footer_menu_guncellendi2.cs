using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ECommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class footer_menu_guncellendi2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SistemMi",
                table: "MenuGruplari",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "MenuGruplari",
                columns: new[] { "Id", "AktifMi", "Baslik", "GuncellenmeTarihi", "Konum", "OlusturulmaTarihi", "SistemMi" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), false, "Footer Kurumsal Menü", null, "FooterKurumsal", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true },
                    { new Guid("22222222-2222-2222-2222-222222222222"), false, "Footer Hızlı Erişim Menü", null, "Footer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MenuGruplari",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "MenuGruplari",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DropColumn(
                name: "SistemMi",
                table: "MenuGruplari");
        }
    }
}
