using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities;

public class Siparis : TemelVarlik
{
    public string SiparisNumarasi { get; set; } = string.Empty;
    public DateTime SiparisTarihi { get; set; } = DateTime.UtcNow;

    // Finansal Alanlar
    public decimal AraToplam { get; set; }
    public decimal TeslimatToplamUcreti { get; set; }
    public decimal VipToplamUcreti { get; set; }
    public decimal KampanyaIndirimTutari { get; set; }
    public string? UygulananKampanyaAdi { get; set; }
    public decimal KuponIndirimTutari { get; set; }
    public string? KullanilanKuponKodu { get; set; }
    public decimal ToplamTutar { get; set; } // Genel Toplam

    public SiparisDurumu Durum { get; set; } = SiparisDurumu.Beklemede;

    // Fatura ve Genel İletişim Bilgileri
    public string FaturaAdresi { get; set; } = string.Empty;
    public string FaturaTipi { get; set; } = "Bireysel";
    public string? FirmaAdi { get; set; }
    public string? VergiNo { get; set; }

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

    public string MusteriAd { get; set; } = string.Empty;
    public string MusteriSoyad { get; set; } = string.Empty;
    public string MusteriEmail { get; set; } = string.Empty;
    public string MusteriTelefon { get; set; } = string.Empty;
    public Guid? KullaniciId { get; set; }
    public virtual UygulamaKullanicisi? Kullanici { get; set; }

    public virtual ICollection<SiparisKalemi> SiparisKalemleri { get; set; } = new List<SiparisKalemi>();
    public virtual SiparisGorselOnay? GorselOnay { get; set; }
}