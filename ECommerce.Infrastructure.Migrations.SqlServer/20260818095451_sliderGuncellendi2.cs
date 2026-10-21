using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class sliderGuncellendi2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GorselUrl",
                table: "Sliderlar");

            migrationBuilder.RenameColumn(
                name: "HedefUrl",
                table: "Sliderlar",
                newName: "ResimUrl");

            migrationBuilder.RenameColumn(
                name: "Aciklama",
                table: "Sliderlar",
                newName: "LinkUrl");

            migrationBuilder.AddColumn<string>(
                name: "AltMetin",
                table: "Sliderlar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ButonMetni",
                table: "Sliderlar",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AltMetin",
                table: "Sliderlar");

            migrationBuilder.DropColumn(
                name: "ButonMetni",
                table: "Sliderlar");

            migrationBuilder.RenameColumn(
                name: "ResimUrl",
                table: "Sliderlar",
                newName: "HedefUrl");

            migrationBuilder.RenameColumn(
                name: "LinkUrl",
                table: "Sliderlar",
                newName: "Aciklama");

            migrationBuilder.AddColumn<string>(
                name: "GorselUrl",
                table: "Sliderlar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
