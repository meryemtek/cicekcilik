using ECommerce.Application.DTOs.Kategori;

namespace ECommerce.WebUI.Models;

public class HeaderMenuGrupViewModel
{
    public Guid Id { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string SeoUrl { get; set; } = string.Empty;
    public string IconClass { get; set; } = string.Empty;
    public List<KategoriListeDto> AltMenuler { get; set; } = new();
}

public class HeaderMenuYonetimViewModel
{
    public List<HeaderMenuGrupViewModel> AnaMenuler { get; set; } = new();
}
