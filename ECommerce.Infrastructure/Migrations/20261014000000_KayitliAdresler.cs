using System;
using ECommerce.Infrastructure.Context;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Migrations
{
    [DbContext(typeof(UygulamaDbContext))]
    [Migration("20261014000000_KayitliAdresler")]
    public partial class KayitliAdresler : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KayitliAdresler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KullaniciId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    AliciAdSoyad = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Il = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Ilce = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    AcikAdres = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    VarsayilanMi = table.Column<bool>(type: "bit", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KayitliAdresler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KayitliAdresler_AspNetUsers_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KayitliAdresler_KullaniciId",
                table: "KayitliAdresler",
                column: "KullaniciId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "KayitliAdresler");
        }
    }
}
