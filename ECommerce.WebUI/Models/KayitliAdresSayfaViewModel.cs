using ECommerce.Application.DTOs.Adres;

namespace ECommerce.WebUI.Models;

public class KayitliAdresSayfaViewModel
{
    public List<KayitliAdresDto> Adresler { get; set; } = new();
    public KayitliAdresKaydetDto Form { get; set; } = new();
    public Guid? DuzenlenenId { get; set; }
}
