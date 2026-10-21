using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHakkimizdaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HakkimizdaAyarlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HikayemizAktifMi = table.Column<bool>(type: "bit", nullable: false),
                    HikayemizBaslik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HikayemizMetni = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NedenBiziAktifMi = table.Column<bool>(type: "bit", nullable: false),
                    DegerlerimizAktifMi = table.Column<bool>(type: "bit", nullable: false),
                    DegerlerimizJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HizmetBolgeleriAktifMi = table.Column<bool>(type: "bit", nullable: false),
                    HizmetBolgeleriJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeoTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeoDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HakkimizdaAyarlari", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HakkimizdaAyarlari");
        }
    }
}
