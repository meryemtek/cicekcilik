namespace ECommerce.Domain.Entities;

public class Sayfa : TemelVarlik
{
    public string Baslik { get; set; } = string.Empty;
    public string SeoUrl { get; set; } = string.Empty;
    public string Icerik { get; set; } = string.Empty;
    public string MetaDescription { get; set; } = string.Empty;
    public string MetaKeywords { get; set; } = string.Empty;
    public bool AktifMi { get; set; } = true;
    public string SayfaKodu { get; set; } = string.Empty;
}