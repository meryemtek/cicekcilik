using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MenuGrupAciklamaEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Aciklama",
                table: "MenuGruplari",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "MenuGruplari",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "Aciklama", "AktifMi" },
                values: new object[] { "Şirketiniz ve hizmetleriniz hakkında kısa bir açıklama yazın.", true });

            migrationBuilder.UpdateData(
                table: "MenuGruplari",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "Aciklama", "AktifMi" },
                values: new object[] { "Müşterilerinizin bilmesini istediğiniz kısa bir not yazın.", true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Aciklama",
                table: "MenuGruplari");

            migrationBuilder.UpdateData(
                table: "MenuGruplari",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "AktifMi",
                value: false);

            migrationBuilder.UpdateData(
                table: "MenuGruplari",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "AktifMi",
                value: false);
        }
    }
}
