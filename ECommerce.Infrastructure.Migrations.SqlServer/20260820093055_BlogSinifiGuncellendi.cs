using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BlogSinifiGuncellendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MetaAciklama",
                table: "BlogYazilari",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaBaslik",
                table: "BlogYazilari",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "YayinlanmaTarihi",
                table: "BlogYazilari",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MetaAciklama",
                table: "BlogYazilari");

            migrationBuilder.DropColumn(
                name: "MetaBaslik",
                table: "BlogYazilari");

            migrationBuilder.DropColumn(
                name: "YayinlanmaTarihi",
                table: "BlogYazilari");
        }
    }
}
