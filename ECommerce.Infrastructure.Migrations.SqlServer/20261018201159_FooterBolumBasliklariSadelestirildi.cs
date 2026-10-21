using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FooterBolumBasliklariSadelestirildi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE [MenuGruplari] SET [Baslik] = N'Kurumsal'
WHERE [Id] = '11111111-1111-1111-1111-111111111111' AND [Baslik] = N'Footer Kurumsal Menü';

UPDATE [MenuGruplari] SET [Baslik] = N'Hızlı Erişim'
WHERE [Id] = '22222222-2222-2222-2222-222222222222' AND [Baslik] = N'Footer Hızlı Erişim Menü';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Kullanıcıya görünen bölüm başlıkları rollback sırasında korunur.
        }
    }
}
