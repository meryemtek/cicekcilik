namespace ECommerce.Application.DTOs.Siparis
{
    public class SiparisOlusturDto
    {
        public string SepetId { get; set; } = string.Empty;

        // Müşteri / Gönderici Bilgileri
        public string MusteriAd { get; set; } = string.Empty;
        public string MusteriSoyad { get; set; } = string.Empty;
        public string MusteriEmail { get; set; } = string.Empty;
        public string MusteriTelefon { get; set; } = string.Empty;
        public string FaturaAdresi { get; set; } = string.Empty;
        public string FaturaTipi { get; set; } = "Bireysel";
        public string? FirmaAdi { get; set; }
        public string? VergiNo { get; set; }
        public Guid? KullaniciId { get; set; }

        // Alıcı ve Teslimat Bilgileri
        public string AliciAdSoyad { get; set; } = string.Empty;
        public string AliciTelefon { get; set; } = string.Empty;
        public string GonderimYeriAdi { get; set; } = string.Empty;
        public string TeslimatAdresi { get; set; } = string.Empty;

        // Çiçek Kart Notu ve Gönderen
        public string? KartNotu { get; set; }
        public string? GonderenGorunenAd { get; set; }

        // Ödeme Yöntemi
        public string OdemeYontemi { get; set; } = "KrediKarti";
    }
}