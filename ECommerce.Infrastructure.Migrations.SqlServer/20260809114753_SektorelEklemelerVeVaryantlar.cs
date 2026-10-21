using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SektorelEklemelerVeVaryantlar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AiAciklama",
                table: "Urunler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "TeslimatTarihi",
                table: "Siparisler",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "TeslimatZamanAraligi",
                table: "Siparisler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "VipTeslimatMi",
                table: "Siparisler",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "KartNotuSablonlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Kategori = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MesajIcerigi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiraNo = table.Column<int>(type: "int", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KartNotuSablonlari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SemtTeslimatBolgeleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IlceAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MahalleAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EkTeslimatUcreti = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SemtTeslimatBolgeleri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiparisGorselOnaylari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiparisId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GorselUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YuklemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MusteriOnayladiMi = table.Column<bool>(type: "bit", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiparisGorselOnaylari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiparisGorselOnaylari_Siparisler_SiparisId",
                        column: x => x.SiparisId,
                        principalTable: "Siparisler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UrunVaryantlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UrunId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VaryantAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FiyatFarki = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StokMiktari = table.Column<int>(type: "int", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrunVaryantlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UrunVaryantlari_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SiparisGorselOnaylari_SiparisId",
                table: "SiparisGorselOnaylari",
                column: "SiparisId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UrunVaryantlari_UrunId",
                table: "UrunVaryantlari",
                column: "UrunId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KartNotuSablonlari");

            migrationBuilder.DropTable(
                name: "SemtTeslimatBolgeleri");

            migrationBuilder.DropTable(
                name: "SiparisGorselOnaylari");

            migrationBuilder.DropTable(
                name: "UrunVaryantlari");

            migrationBuilder.DropColumn(
                name: "AiAciklama",
                table: "Urunler");

            migrationBuilder.DropColumn(
                name: "TeslimatTarihi",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "TeslimatZamanAraligi",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "VipTeslimatMi",
                table: "Siparisler");
        }
    }
}
