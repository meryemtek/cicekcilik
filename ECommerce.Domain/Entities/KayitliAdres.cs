namespace ECommerce.Domain.Entities;

public class KayitliAdres : TemelVarlik
{
    public Guid KullaniciId { get; set; }
    public UygulamaKullanicisi Kullanici { get; set; } = null!;
    public string Baslik { get; set; } = string.Empty;
    public string AliciAdSoyad { get; set; } = string.Empty;
    public string Telefon { get; set; } = string.Empty;
    public string Il { get; set; } = string.Empty;
    public string Ilce { get; set; } = string.Empty;
    public string AcikAdres { get; set; } = string.Empty;
    public bool VarsayilanMi { get; set; }
}
