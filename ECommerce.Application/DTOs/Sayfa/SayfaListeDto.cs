namespace ECommerce.Application.DTOs.Sayfa;

public class SayfaListeDto
{
    public Guid Id { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string SayfaKodu { get; set; } = string.Empty;
    public string SeoUrl { get; set; } = string.Empty;
    public bool AktifMi { get; set; }
}