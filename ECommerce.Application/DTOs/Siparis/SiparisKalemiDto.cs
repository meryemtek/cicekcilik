namespace ECommerce.Application.DTOs.Siparis;

public class SiparisKalemiDto
{
    public Guid UrunId { get; set; }
    public string UrunAdi { get; set; } = string.Empty;
    public Guid? VaryantId { get; set; }
    public string VaryantAdi { get; set; } = string.Empty;
    public int Miktar { get; set; }
    public decimal BirimFiyat { get; set; }
    public string GorselUrl { get; set; } = string.Empty;

    public string Il { get; set; } = string.Empty;
    public string Ilce { get; set; } = string.Empty;
    public string Mahalle { get; set; } = string.Empty;
    public string TeslimatGunu { get; set; } = string.Empty;
    public string TeslimatSaati { get; set; } = string.Empty;
    public bool VipTeslimatAktifMi { get; set; }
    public decimal TeslimatUcreti { get; set; }
    public decimal VipUcreti { get; set; }
    public decimal ToplamFiyat { get; set; }
}