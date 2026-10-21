using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUstKategoriToCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UstKategoriId",
                table: "Kategoriler",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kategoriler_UstKategoriId",
                table: "Kategoriler",
                column: "UstKategoriId");

            migrationBuilder.AddForeignKey(
                name: "FK_Kategoriler_Kategoriler_UstKategoriId",
                table: "Kategoriler",
                column: "UstKategoriId",
                principalTable: "Kategoriler",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kategoriler_Kategoriler_UstKategoriId",
                table: "Kategoriler");

            migrationBuilder.DropIndex(
                name: "IX_Kategoriler_UstKategoriId",
                table: "Kategoriler");

            migrationBuilder.DropColumn(
                name: "UstKategoriId",
                table: "Kategoriler");
        }
    }
}
