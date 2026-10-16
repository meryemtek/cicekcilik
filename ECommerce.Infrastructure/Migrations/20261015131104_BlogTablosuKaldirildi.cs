using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BlogTablosuKaldirildi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogYazilari");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlogYazilari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GorselUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Icerik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MetaAciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaBaslik = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OkunmaSayisi = table.Column<int>(type: "int", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ozet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YayinlanmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogYazilari", x => x.Id);
                });
        }
    }
}
