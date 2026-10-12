using ECommerce.Domain.Entities;

public class SiteAyari : TemelVarlik
{
    public string FirmaAdi { get; set; } = string.Empty;
    public string Telefon1 { get; set; } = string.Empty;
    public string? Telefon2 { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Adres { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string TemaRengiHex { get; set; } = string.Empty;
    public string? UrunDetayDuyurulari { get; set; }
    public string? FacebookUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public string? GoogleMapsEmbedKodu { get; set; }
    public decimal VipTeslimatUcreti { get; set; }

    // --- ENTEGRASYON ALANLARI ---
    public string? AdminWhatsAppNumarasi { get; set; }
    public string? NetGsmKullaniciAdi { get; set; }
    public string? NetGsmSifre { get; set; }
    public string? NetGsmBaslik { get; set; }

    public string? PaytrMerchantId { get; set; }
    public string? PaytrMerchantKey { get; set; }
    public string? PaytrMerchantSalt { get; set; }
    public string? OpenAiApiKey { get; set; }

    // --- ÖN MUHASEBE (HİCARİ) ENTEGRASYON ALANLARI ---
    public bool HiCariEntegrasyonAktifMi { get; set; } = false;
    public string? HiCariApiUrl { get; set; } = string.Empty;
    public string? HiCariApiKey { get; set; } = string.Empty;
}