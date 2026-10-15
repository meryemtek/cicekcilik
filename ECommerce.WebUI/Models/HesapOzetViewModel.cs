using ECommerce.Application.DTOs.Siparis;

namespace ECommerce.WebUI.Models;

public class HesapOzetViewModel
{
    public string AdSoyad { get; set; } = string.Empty;
    public int SiparisSayisi { get; set; }
    public int FavoriSayisi { get; set; }
    public int AdresSayisi { get; set; }
    public SiparisListeDto? SonSiparis { get; set; }
}
