using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class KurumsalVeSepetTablolariEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Siparisler_AspNetUsers_KullaniciId",
                table: "Siparisler");

            migrationBuilder.AlterColumn<Guid>(
                name: "KullaniciId",
                table: "Siparisler",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "FaturaAdresi",
                table: "Siparisler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MusteriAd",
                table: "Siparisler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MusteriEmail",
                table: "Siparisler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MusteriSoyad",
                table: "Siparisler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MusteriTelefon",
                table: "Siparisler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "BankaHesaplari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankaAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubeKodu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HesapNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Iban = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AliciAdSoyad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankaHesaplari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlogYazilari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ozet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Icerik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GorselUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OkunmaSayisi = table.Column<int>(type: "int", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogYazilari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IletisimMesajlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GonderenAdSoyad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GonderenEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Konu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mesaj = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OkunduMu = table.Column<bool>(type: "bit", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IletisimMesajlari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kuponlar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Kod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IndirimMiktari = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    YuzdelikMi = table.Column<bool>(type: "bit", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kuponlar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sayfalar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Icerik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    SayfaKodu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sayfalar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SikSorulanSorular",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Soru = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cevap = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiraNo = table.Column<int>(type: "int", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SikSorulanSorular", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteAyarlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirmaAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefon1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefon2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adres = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TemaRengiHex = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FacebookUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstagramUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TwitterUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GoogleMapsEmbedKodu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteAyarlari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sliderlar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GorselUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HedefUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SiraNo = table.Column<int>(type: "int", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sliderlar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UrunYorumlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UrunId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    YorumcuAd = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YorumMetni = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Puan = table.Column<byte>(type: "tinyint", nullable: false),
                    OnaylandiMi = table.Column<bool>(type: "bit", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrunYorumlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UrunYorumlari_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UrunYorumlari_UrunId",
                table: "UrunYorumlari",
                column: "UrunId");

            migrationBuilder.AddForeignKey(
                name: "FK_Siparisler_AspNetUsers_KullaniciId",
                table: "Siparisler",
                column: "KullaniciId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Siparisler_AspNetUsers_KullaniciId",
                table: "Siparisler");

            migrationBuilder.DropTable(
                name: "BankaHesaplari");

            migrationBuilder.DropTable(
                name: "BlogYazilari");

            migrationBuilder.DropTable(
                name: "IletisimMesajlari");

            migrationBuilder.DropTable(
                name: "Kuponlar");

            migrationBuilder.DropTable(
                name: "Sayfalar");

            migrationBuilder.DropTable(
                name: "SikSorulanSorular");

            migrationBuilder.DropTable(
                name: "SiteAyarlari");

            migrationBuilder.DropTable(
                name: "Sliderlar");

            migrationBuilder.DropTable(
                name: "UrunYorumlari");

            migrationBuilder.DropColumn(
                name: "FaturaAdresi",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "MusteriAd",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "MusteriEmail",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "MusteriSoyad",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "MusteriTelefon",
                table: "Siparisler");

            migrationBuilder.AlterColumn<Guid>(
                name: "KullaniciId",
                table: "Siparisler",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Siparisler_AspNetUsers_KullaniciId",
                table: "Siparisler",
                column: "KullaniciId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
