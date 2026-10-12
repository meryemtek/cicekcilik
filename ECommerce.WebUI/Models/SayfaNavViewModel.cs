namespace ECommerce.WebUI.Models;

public class SayfaNavViewModel
{
    public List<ECommerce.Application.DTOs.Sayfa.SayfaListeDto> Sayfalar { get; set; } = new();
    public string DestekTelefonu { get; set; } = "0850 000 00 00"; // Varsayılan yedek numara
}