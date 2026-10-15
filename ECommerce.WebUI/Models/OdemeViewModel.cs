using ECommerce.Application.DTOs.BankaHesabi;
using ECommerce.Application.DTOs.KartNotuSablonu;
using ECommerce.Application.Models;

namespace ECommerce.WebUI.Models
{
    public class OdemeViewModel
    {
        public Sepet Sepet { get; set; } = new();
        public List<KartNotuSablonuListeDto> KartNotlari { get; set; } = new();
        public List<BankaHesabiPublicDto> Bankalar { get; set; } = new();
        public List<ECommerce.Application.DTOs.Adres.KayitliAdresDto> KayitliAdresler { get; set; } = new();

        // Sabit sayfa başlık ve içerikleri
        public string MesafeliSatisBaslik { get; set; } = "Mesafeli Satış Sözleşmesi";
        public string MesafeliSatisIcerigi { get; set; } = string.Empty;

        public string GizlilikBaslik { get; set; } = "Gizlilik Politikası";
        public string GizlilikPolitikasiIcerigi { get; set; } = string.Empty;
    }
}
