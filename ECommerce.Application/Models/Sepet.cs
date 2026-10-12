namespace ECommerce.Application.Models;

public class Sepet
{
    public string SepetId { get; set; } = string.Empty;
    public List<SepetKalemi> Kalemler { get; set; } = new List<SepetKalemi>();

    // Kupon Bilgileri
    public string? IndirimKodu { get; set; }
    public decimal IndirimTutari { get; set; }
    public decimal KampanyaIndirimTutari { get; set; }
    public string? UygulananKampanyaAdi { get; set; }

    // Ödeme ve Sepet sayfalarının kullandığı Toplam Tutar Hesabı
    public decimal ToplamTutar =>
        Kalemler.Sum(k => (k.BirimFiyat * k.Miktar) + k.TeslimatUcreti + k.VipUcreti) - IndirimTutari;

    public Sepet()
    {
    }

    public Sepet(string sepetId)
    {
        SepetId = sepetId;
    }
}