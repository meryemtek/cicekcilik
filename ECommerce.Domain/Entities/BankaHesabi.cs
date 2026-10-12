namespace ECommerce.Domain.Entities;

public class BankaHesabi : TemelVarlik
{
    public string BankaAdi { get; set; } = string.Empty;
    public string SubeKodu { get; set; } = string.Empty;
    public string HesapNo { get; set; } = string.Empty;
    public string Iban { get; set; } = string.Empty;
    public string AliciAdSoyad { get; set; } = string.Empty;
    public bool AktifMi { get; set; } = true;
}