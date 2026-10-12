namespace ECommerce.Application.DTOs.Urun;

public class TopluFiyatGuncelleDto
{
    public decimal OranVeyaMiktar { get; set; } // Örn: 10 (Yüzde 10 artış için) veya 50 (50 TL artış için)
    public bool YuzdelikMi { get; set; } // True ise yüzde, False ise net tutar
    public bool ArtisMi { get; set; } // True ise zam (artış), False ise indirim (azalış)
    public Guid? KategoriId { get; set; } // Eğer boş bırakılırsa TÜM ürünlere uygulanır
}