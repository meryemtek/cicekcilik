using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class VarsayilanTeslimatBolgeleriEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM [SemtTeslimatBolgeleri])
BEGIN
    INSERT INTO [SemtTeslimatBolgeleri] ([Id], [IlAdi], [IlceAdi], [MahalleAdi], [EkTeslimatUcreti], [AktifMi], [OlusturulmaTarihi])
    SELECT NEWID(), N'Elazığ', N'Merkez', [MahalleAdi], [EkTeslimatUcreti], 1, SYSUTCDATETIME()
    FROM (VALUES
        (N'Ataşehir', CAST(0 AS decimal(18, 2))),
        (N'Çaydaçıra', CAST(0 AS decimal(18, 2))),
        (N'Sürsürü', CAST(0 AS decimal(18, 2))),
        (N'Abdullahpaşa', CAST(0 AS decimal(18, 2))),
        (N'Cumhuriyet', CAST(0 AS decimal(18, 2))),
        (N'Doğukent', CAST(100 AS decimal(18, 2))),
        (N'Bahçelievler', CAST(0 AS decimal(18, 2))),
        (N'Üniversite', CAST(0 AS decimal(18, 2))),
        (N'İzzetpaşa', CAST(0 AS decimal(18, 2))),
        (N'Fırat', CAST(0 AS decimal(18, 2))),
        (N'Rızaiye', CAST(0 AS decimal(18, 2))),
        (N'Yenimahalle', CAST(0 AS decimal(18, 2))),
        (N'Hankendi', CAST(0 AS decimal(18, 2)))
    ) AS [Varsayilanlar]([MahalleAdi], [EkTeslimatUcreti]);
END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Varsayılan teslimat bölgeleri rollback sırasında korunur.
        }
    }
}
