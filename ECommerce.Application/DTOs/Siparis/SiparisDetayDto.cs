using ECommerce.Application.DTOs.Siparis;
using ECommerce.Domain.Enums;

namespace ECommerce.Application.DTOs.Siparis
{
    public class SiparisDetayDto
    {
        public Guid Id { get; set; }
        public string SiparisNumarasi { get; set; } = string.Empty;
        public DateTime SiparisTarihi { get; set; }
        public decimal AraToplam { get; set; }
        public decimal TeslimatToplamUcreti { get; set; }
        public decimal VipToplamUcreti { get; set; }
        public decimal KampanyaIndirimTutari { get; set; }
        public string? UygulananKampanyaAdi { get; set; }
        public decimal KuponIndirimTutari { get; set; }
        public string? KullanilanKuponKodu { get; set; }
        public decimal ToplamTutar { get; set; }
        public SiparisDurumu Durum { get; set; }
        public string FaturaTipi { get; set; } = "Bireysel";
        public string? FirmaAdi { get; set; }
        public string? VergiNo { get; set; }
        public string AliciAdSoyad { get; set; } = string.Empty;
        public string AliciTelefon { get; set; } = string.Empty;
        public string GonderimYeriAdi { get; set; } = string.Empty;
        public string TeslimatAdresi { get; set; } = string.Empty;
        public string FaturaAdresi { get; set; } = string.Empty;
        public string? KartNotu { get; set; }
        public string? GonderenGorunenAd { get; set; }
        public string OdemeYontemi { get; set; } = "KrediKarti";
        public string MusteriAd { get; set; } = string.Empty;
        public string MusteriSoyad { get; set; } = string.Empty;
        public string MusteriEmail { get; set; } = string.Empty;
        public string MusteriTelefon { get; set; } = string.Empty;
        public List<SiparisKalemiDto> Kalemler { get; set; } = new List<SiparisKalemiDto>();
    }

}