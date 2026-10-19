using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FooterKurumsalBolumuEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM [MenuGruplari] WHERE [Id] = '11111111-1111-1111-1111-111111111111')
BEGIN
    INSERT INTO [MenuGruplari] ([Id], [Baslik], [Aciklama], [Konum], [AktifMi], [SistemMi], [OlusturulmaTarihi])
    VALUES ('11111111-1111-1111-1111-111111111111', N'Kurumsal', N'Şirketiniz ve hizmetleriniz hakkında kısa bir açıklama yazın.', N'FooterKurumsal', 1, 1, '2026-01-01T00:00:00');
END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Varsayılan sistem bölümü verisi geri alma sırasında korunur.
        }
    }
}
