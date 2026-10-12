using ECommerce.Application.DTOs.IletisimMesaji;
using ECommerce.Application.DTOs.Menu;

namespace ECommerce.WebUI.Models
{
    public class IletisimViewModel
    {
        public IletisimMesajiEkleDto Dto { get; set; } = new();

        // Sol taraf için Site Ayarları verileri
        public string FirmaAdi { get; set; } = string.Empty;
        public string Adres { get; set; } = string.Empty;
        public string Telefon1 { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string InstagramUrl { get; set; } = string.Empty;
        public string FacebookUrl { get; set; } = string.Empty;
        public string TwitterUrl { get; set; } = string.Empty;
    }
}