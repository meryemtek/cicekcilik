using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class WordPressTarziMenuSistemiEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItems_MenuItems_ParentId",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "AktifMi",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "Konum",
                table: "MenuItems");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "MenuItems",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Baslik",
                table: "MenuItems",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "MenuGrupId",
                table: "MenuItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "MenuGruplari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Konum = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuGruplari", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_MenuGrupId",
                table: "MenuItems",
                column: "MenuGrupId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItems_MenuGruplari_MenuGrupId",
                table: "MenuItems",
                column: "MenuGrupId",
                principalTable: "MenuGruplari",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItems_MenuItems_ParentId",
                table: "MenuItems",
                column: "ParentId",
                principalTable: "MenuItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItems_MenuGruplari_MenuGrupId",
                table: "MenuItems");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuItems_MenuItems_ParentId",
                table: "MenuItems");

            migrationBuilder.DropTable(
                name: "MenuGruplari");

            migrationBuilder.DropIndex(
                name: "IX_MenuItems_MenuGrupId",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "MenuGrupId",
                table: "MenuItems");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "MenuItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "Baslik",
                table: "MenuItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AddColumn<bool>(
                name: "AktifMi",
                table: "MenuItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Konum",
                table: "MenuItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItems_MenuItems_ParentId",
                table: "MenuItems",
                column: "ParentId",
                principalTable: "MenuItems",
                principalColumn: "Id");
        }
    }
}
