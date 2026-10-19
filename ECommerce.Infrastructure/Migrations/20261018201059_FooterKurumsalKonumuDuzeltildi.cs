using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FooterKurumsalKonumuDuzeltildi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM [MenuGruplari] WHERE [Id] = '11111111-1111-1111-1111-111111111111')
BEGIN
    UPDATE [MenuGruplari]
    SET [Konum] = N'FooterKurumsal',
        [Aciklama] = COALESCE([Aciklama], N'Şirketiniz ve hizmetleriniz hakkında kısa bir açıklama yazın.'),
        [AktifMi] = 1
    WHERE [Id] = '11111111-1111-1111-1111-111111111111';
END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Footer bölümü ayarları rollback sırasında korunur.
        }
    }
}
