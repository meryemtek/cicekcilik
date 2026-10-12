namespace ECommerce.Application.DTOs.Sepet;

public class SepetEkleDto
{
    public Guid UrunId { get; set; }
    public int Adet { get; set; } = 1;
    public Guid? VaryantId { get; set; }

    // Çiçekçilik Operasyonel Teslimat Tercihleri
    public string Il { get; set; } = string.Empty;
    public string Ilce { get; set; } = string.Empty;
    public string Mahalle { get; set; } = string.Empty;
    public string TeslimatGunu { get; set; } = string.Empty;
    public string TeslimatSaati { get; set; } = string.Empty;
    public bool VipTeslimatAktifMi { get; set; }
    public decimal MahalleFarki { get; set; } // Teslimat ücretini formdan alabilmek için

    // Ek Ücretlerin Dinamik Aktarımı İçin:
    public decimal VipFarki { get; set; }
}
