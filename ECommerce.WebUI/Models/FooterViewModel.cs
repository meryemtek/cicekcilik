namespace ECommerce.WebUI.Models;

public class FooterViewModel
{
    public string FirmaAdi { get; set; } = "Çiçekçilik";
    public string LogoUrl { get; set; } = "";
    public string Telefon1 { get; set; } = "";
    public string AdminWhatsAppNumarasi { get; set; } = "";
    public string Email { get; set; } = "";
    public string Adres { get; set; } = "";
    public string InstagramUrl { get; set; } = "";
    public string? FacebookUrl { get; set; } = "";
    public string? TwitterUrl { get; set; } = "";
    public List<ECommerce.Application.DTOs.Sayfa.SayfaListeDto> Sayfalar { get; set; } = new();
    public List<ECommerce.Application.DTOs.Menu.MenuGrupListeDto> FooterBolumleri { get; set; } = new();
}
