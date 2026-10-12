namespace ECommerce.Application.DTOs.BankaHesabi;

public class BankaHesabiEkleDto
{
    public string BankaAdi { get; set; } = string.Empty;
    public string SubeKodu { get; set; } = string.Empty;
    public string HesapNo { get; set; } = string.Empty;
    public string Iban { get; set; } = string.Empty;
    public string AliciAdSoyad { get; set; } = string.Empty;
    public bool AktifMi { get; set; } = true;
}