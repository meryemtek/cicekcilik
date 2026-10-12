namespace ECommerce.Application.DTOs.Sepet;

public class SepetKalemDto
{
    public Guid UrunId { get; set; }
    public string UrunAdi { get; set; } = string.Empty;
    public Guid? VaryantId { get; set; }
    public string VaryantAdi { get; set; } = string.Empty; // Örn: Büyük Boy (Lux)
    public int Adet { get; set; }
    public string GorselUrl { get; set; } = string.Empty;

    public decimal BirimFiyat { get; set; } // Bu artık SADECE Ürün Fiyatı + Varyant Farkı olacak
    public decimal TeslimatUcreti { get; set; } // Mahalle ücreti buraya gelecek
    public decimal VipUcreti { get; set; } 

    // Sepet kaleminin genel toplamı
    public decimal ToplamFiyat => (BirimFiyat * Adet) + TeslimatUcreti + VipUcreti;
}