using ECommerce.Domain.Enums;

namespace ECommerce.Application.DTOs.Siparis;

public static class SiparisDurumuMetinleri
{
    public static string GorunumAdi(this SiparisDurumu durum) => durum switch
    {
        SiparisDurumu.Beklemede => "Beklemede",
        SiparisDurumu.Islemde => "Hazırlanıyor",
        SiparisDurumu.Kargoda => "Kargoya verildi",
        SiparisDurumu.TeslimEdildi => "Teslim Edildi",
        SiparisDurumu.IptalEdildi => "İptal Edildi",
        SiparisDurumu.TeslimEdilemedi => "Teslim edilemedi",
        _ => "Bilinmeyen durum"
    };
}
