using ECommerce.Application.DTOs.Siparis;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using System.Net;

public class SiparisServisi : ISiparisServisi
{
    private readonly ISiparisDeposu _siparisDeposu; // Özel depo enjekte ediliyor
    private readonly ISepetServisi _sepetServisi;
    private readonly ISmsServisi _smsServisi;
    private readonly IEmailServisi _emailServisi;
    private readonly IHiCariEntegrasyonServisi _hiCariServisi;
    private readonly IDepo<Urun> _urunDeposu;
    private readonly IDepo<UrunVaryant> _varyantDeposu;

    public SiparisServisi(
        ISiparisDeposu siparisDeposu,
        ISepetServisi sepetServisi,
        ISmsServisi smsServisi,
        IEmailServisi emailServisi,
        IHiCariEntegrasyonServisi hiCariServisi,
        IDepo<Urun> urunDeposu,
        IDepo<UrunVaryant> varyantDeposu)
    {
        _siparisDeposu = siparisDeposu;
        _sepetServisi = sepetServisi;
        _smsServisi = smsServisi;
        _emailServisi = emailServisi;
        _hiCariServisi = hiCariServisi;
        _urunDeposu = urunDeposu;
        _varyantDeposu = varyantDeposu;
    }

    public async Task<IReadOnlyList<SiparisListeDto>> TumSiparisleriGetirAsync()
    {
        var siparisler = await _siparisDeposu.TumunuGetirAsync();

        return siparisler.Select(s => new SiparisListeDto
        {
            Id = s.Id,
            SiparisNumarasi = s.SiparisNumarasi,
            SiparisTarihi = s.SiparisTarihi,
            ToplamTutar = s.ToplamTutar,
            Durum = s.Durum,
            MusteriAdSoyad = $"{s.MusteriAd} {s.MusteriSoyad}"
        }).OrderByDescending(s => s.SiparisTarihi).ToList();
    }

    public async Task<IReadOnlyList<SiparisListeDto>> KullanicininSiparisleriniGetirAsync(Guid kullaniciId)
    {
        var siparisler = await _siparisDeposu.GetirAsync(s => s.KullaniciId == kullaniciId);

        return siparisler.Select(s => new SiparisListeDto
        {
            Id = s.Id,
            SiparisNumarasi = s.SiparisNumarasi,
            SiparisTarihi = s.SiparisTarihi,
            ToplamTutar = s.ToplamTutar,
            Durum = s.Durum,
            MusteriAdSoyad = $"{s.MusteriAd} {s.MusteriSoyad}"
        }).OrderByDescending(s => s.SiparisTarihi).ToList();
    }

    public async Task<SiparisDetayDto?> SiparisDetayGetirAsync(Guid id)
    {
        // Özel depo üzerinden kalemler ve ürünler dahil edilerek getiriliyor
        var siparis = await _siparisDeposu.SiparisDetayiGetirAsync(id);
        if (siparis == null) return null;

        return new SiparisDetayDto
        {
            Id = siparis.Id,
            SiparisNumarasi = siparis.SiparisNumarasi,
            SiparisTarihi = siparis.SiparisTarihi,
            AraToplam = siparis.AraToplam,
            TeslimatToplamUcreti = siparis.TeslimatToplamUcreti,
            VipToplamUcreti = siparis.VipToplamUcreti,
            KampanyaIndirimTutari = siparis.KampanyaIndirimTutari,
            UygulananKampanyaAdi = siparis.UygulananKampanyaAdi,
            KuponIndirimTutari = siparis.KuponIndirimTutari,
            KullanilanKuponKodu = siparis.KullanilanKuponKodu,
            ToplamTutar = siparis.ToplamTutar,
            Durum = siparis.Durum,

            FaturaTipi = siparis.FaturaTipi,
            FirmaAdi = siparis.FirmaAdi,
            VergiNo = siparis.VergiNo,
            AliciAdSoyad = siparis.AliciAdSoyad,
            AliciTelefon = siparis.AliciTelefon,
            GonderimYeriAdi = siparis.GonderimYeriAdi,
            TeslimatAdresi = siparis.TeslimatAdresi,
            FaturaAdresi = siparis.FaturaAdresi,
            KartNotu = siparis.KartNotu,
            GonderenGorunenAd = siparis.GonderenGorunenAd,
            OdemeYontemi = siparis.OdemeYontemi,

            MusteriAd = siparis.MusteriAd,
            MusteriSoyad = siparis.MusteriSoyad,
            MusteriEmail = siparis.MusteriEmail,
            MusteriTelefon = siparis.MusteriTelefon,

            Kalemler = siparis.SiparisKalemleri != null ? siparis.SiparisKalemleri.Select(k => new SiparisKalemiDto
            {
                UrunId = k.UrunId,
                UrunAdi = k.Urun?.Ad ?? string.Empty,
                VaryantId = k.VaryantId,
                VaryantAdi = k.VaryantAdi,
                Miktar = k.Miktar,
                BirimFiyat = k.BirimFiyat,
                GorselUrl = k.GorselUrl,
                Il = k.Il,
                Ilce = k.Ilce,
                Mahalle = k.Mahalle,
                TeslimatGunu = k.TeslimatGunu,
                TeslimatSaati = k.TeslimatSaati,
                VipTeslimatAktifMi = k.VipTeslimatAktifMi,
                TeslimatUcreti = k.TeslimatUcreti,
                VipUcreti = k.VipUcreti,
                ToplamFiyat = k.ToplamFiyat
            }).ToList() : new List<SiparisKalemiDto>()
        };
    }

    public async Task<Guid> SiparisOlusturAsync(SiparisOlusturDto dto)
    {
        var sepet = await _sepetServisi.SepetFiyatlariniGuncelleAsync(dto.SepetId);
        if (sepet == null || !sepet.Kalemler.Any())
            throw new InvalidOperationException("Sepetiniz boş.");

        // 1. Stok ve Aktiflik Kontrolü
        foreach (var k in sepet.Kalemler)
        {
            var urun = await _urunDeposu.IdIleGetirAsync(k.UrunId);
            if (urun == null)
                throw new InvalidOperationException($"'{k.UrunAdi}' adlı ürün sistemde bulunamadı.");

            if (!urun.AktifMi || urun.StokMiktari <= 0)
                throw new InvalidOperationException($"'{urun.Ad}' ürünü stokta tükenmiştir veya satışa kapalıdır.");

            if (urun.StokMiktari < k.Miktar)
                throw new InvalidOperationException($"'{urun.Ad}' ürünü için yeterli stok bulunmamaktadır (Mevcut Stok: {urun.StokMiktari}, İstenen: {k.Miktar}).");

            if (k.VaryantId.HasValue && k.VaryantId.Value != Guid.Empty)
            {
                var varyant = await _varyantDeposu.IdIleGetirAsync(k.VaryantId.Value);
                if (varyant != null)
                {
                    if (!varyant.AktifMi || varyant.StokMiktari <= 0)
                        throw new InvalidOperationException($"'{urun.Ad}' ürününün '{varyant.VaryantAdi}' seçeneği tükenmiştir.");

                    if (varyant.StokMiktari < k.Miktar)
                        throw new InvalidOperationException($"'{urun.Ad}' ({varyant.VaryantAdi}) için yeterli stok bulunmamaktadır (Mevcut Stok: {varyant.StokMiktari}, İstenen: {k.Miktar}).");
                }
            }
        }

        decimal araToplam = sepet.Kalemler.Sum(k => k.BirimFiyat * k.Miktar);
        decimal teslimatToplam = sepet.Kalemler.Sum(k => k.TeslimatUcreti);
        decimal vipToplam = sepet.Kalemler.Sum(k => k.VipUcreti);

        decimal kampanyaIndirimi = sepet.KampanyaIndirimTutari;
        decimal kuponIndirimi = sepet.IndirimTutari;

        decimal genelToplam = araToplam + teslimatToplam + vipToplam - kampanyaIndirimi - kuponIndirimi;
        if (genelToplam < 0) genelToplam = 0;

        var siparisNo = "SP-" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "-" + Guid.NewGuid().ToString("N")[..8];

        var yeniSiparis = new Siparis
        {
            SiparisNumarasi = siparisNo,
            SepetId = dto.SepetId,
            AraToplam = araToplam,
            TeslimatToplamUcreti = teslimatToplam,
            VipToplamUcreti = vipToplam,
            KampanyaIndirimTutari = kampanyaIndirimi,
            UygulananKampanyaAdi = sepet.UygulananKampanyaAdi,
            KuponIndirimTutari = kuponIndirimi,
            KullanilanKuponKodu = sepet.IndirimKodu,
            ToplamTutar = genelToplam,
            Durum = SiparisDurumu.Beklemede,

            FaturaTipi = dto.FaturaTipi,
            FirmaAdi = dto.FirmaAdi,
            VergiNo = dto.VergiNo,
            AliciAdSoyad = dto.AliciAdSoyad,
            AliciTelefon = dto.AliciTelefon,
            GonderimYeriAdi = dto.GonderimYeriAdi,
            TeslimatAdresi = dto.TeslimatAdresi,
            FaturaAdresi = dto.FaturaAdresi,
            KartNotu = dto.KartNotu,
            GonderenGorunenAd = dto.GonderenGorunenAd,
            OdemeYontemi = dto.OdemeYontemi,

            MusteriAd = dto.MusteriAd,
            MusteriSoyad = dto.MusteriSoyad,
            MusteriEmail = dto.MusteriEmail,
            MusteriTelefon = dto.MusteriTelefon,
            KullaniciId = dto.KullaniciId,

            SiparisKalemleri = sepet.Kalemler.Select(k => new SiparisKalemi
            {
                UrunId = k.UrunId,
                VaryantId = k.VaryantId,
                VaryantAdi = k.VaryantAdi,
                Miktar = k.Miktar,
                BirimFiyat = k.BirimFiyat,
                GorselUrl = k.GorselUrl,
                Il = k.Il,
                Ilce = k.Ilce,
                Mahalle = k.Mahalle,
                TeslimatGunu = k.TeslimatGunu,
                TeslimatSaati = k.TeslimatSaati,
                VipTeslimatAktifMi = k.VipTeslimatAktifMi,
                TeslimatUcreti = k.TeslimatUcreti,
                VipUcreti = k.VipUcreti
            }).ToList()
        };

        await _siparisDeposu.EkleAsync(yeniSiparis);
        await SiparisStogunuGuncelleAsync(yeniSiparis, -1);
        await _siparisDeposu.DegisiklikleriKaydetAsync();

        if (!KrediKartiIleOdemeMi(yeniSiparis))
        {
            await _sepetServisi.SepetiTemizleAsync(dto.SepetId);
        }

        await _smsServisi.SmsGonderAsync(dto.MusteriTelefon, $"Sayin {dto.MusteriAd} {dto.MusteriSoyad}, {yeniSiparis.SiparisNumarasi} numarali siparisiniz basariyla alinmistir.");
        await _hiCariServisi.CariVeStokIsleAsync(yeniSiparis.Id);

        return yeniSiparis.Id;
    }

    public async Task<bool> PaytrSonucunuIsleAsync(string siparisNumarasi, string durum, long toplamTutar)
    {
        var siparis = await _siparisDeposu.SiparisNumarasiylaDetayGetirAsync(siparisNumarasi);
        if (siparis == null || !KrediKartiIleOdemeMi(siparis))
        {
            return false;
        }

        if (siparis.Durum == SiparisDurumu.Islemde || siparis.Durum == SiparisDurumu.IptalEdildi)
        {
            return true;
        }

        if (siparis.Durum != SiparisDurumu.Beklemede)
        {
            return false;
        }

        if (durum.Equals("success", StringComparison.OrdinalIgnoreCase))
        {
            var beklenenTutar = decimal.ToInt64(decimal.Round(siparis.ToplamTutar * 100m, 0, MidpointRounding.AwayFromZero));
            if (toplamTutar < beklenenTutar)
            {
                return false;
            }

            siparis.Durum = SiparisDurumu.Islemde;
            siparis.GuncellenmeTarihi = DateTime.UtcNow;
            _siparisDeposu.Guncelle(siparis);
            await _siparisDeposu.DegisiklikleriKaydetAsync();

            if (!string.IsNullOrWhiteSpace(siparis.SepetId))
            {
                await _sepetServisi.SepetiTemizleAsync(siparis.SepetId);
            }

            return true;
        }

        if (!durum.Equals("failed", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        await SiparisStogunuGuncelleAsync(siparis, 1);
        siparis.Durum = SiparisDurumu.IptalEdildi;
        siparis.GuncellenmeTarihi = DateTime.UtcNow;
        _siparisDeposu.Guncelle(siparis);
        await _siparisDeposu.DegisiklikleriKaydetAsync();
        return true;
    }

    public async Task OdemeBaslatmaIptaliniIsleAsync(Guid siparisId)
    {
        var siparis = await _siparisDeposu.SiparisDetayiGetirAsync(siparisId)
            ?? throw new KeyNotFoundException("Sipariş bulunamadı.");

        if (!KrediKartiIleOdemeMi(siparis) || siparis.Durum != SiparisDurumu.Beklemede)
        {
            return;
        }

        await SiparisStogunuGuncelleAsync(siparis, 1);
        siparis.Durum = SiparisDurumu.IptalEdildi;
        siparis.GuncellenmeTarihi = DateTime.UtcNow;
        _siparisDeposu.Guncelle(siparis);
        await _siparisDeposu.DegisiklikleriKaydetAsync();
    }

    private static bool KrediKartiIleOdemeMi(Siparis siparis)
    {
        return string.Equals(siparis.OdemeYontemi, "KrediKarti", StringComparison.OrdinalIgnoreCase);
    }

    private async Task SiparisStogunuGuncelleAsync(Siparis siparis, int carpan)
    {
        foreach (var kalem in siparis.SiparisKalemleri)
        {
            var urun = await _urunDeposu.IdIleGetirAsync(kalem.UrunId);
            if (urun != null)
            {
                urun.StokMiktari = Math.Max(0, urun.StokMiktari + carpan * kalem.Miktar);
                if (carpan < 0 && urun.StokMiktari == 0)
                {
                    urun.AktifMi = false;
                }
                else if (carpan > 0 && urun.StokMiktari > 0)
                {
                    urun.AktifMi = true;
                }
                urun.GuncellenmeTarihi = DateTime.UtcNow;
                _urunDeposu.Guncelle(urun);
            }

            if (kalem.VaryantId.HasValue && kalem.VaryantId.Value != Guid.Empty)
            {
                var varyant = await _varyantDeposu.IdIleGetirAsync(kalem.VaryantId.Value);
                if (varyant != null)
                {
                    varyant.StokMiktari = Math.Max(0, varyant.StokMiktari + carpan * kalem.Miktar);
                    if (carpan < 0 && varyant.StokMiktari == 0)
                    {
                        varyant.AktifMi = false;
                    }
                    else if (carpan > 0 && varyant.StokMiktari > 0)
                    {
                        varyant.AktifMi = true;
                    }
                    varyant.GuncellenmeTarihi = DateTime.UtcNow;
                    _varyantDeposu.Guncelle(varyant);
                }
            }
        }
    }

    public async Task<SiparisDurumGuncelleSonucDto> DurumGuncelleAsync(SiparisDurumGuncelleDto dto)
    {
        if (!dto.YeniDurum.HasValue || !Enum.IsDefined(dto.YeniDurum.Value)
            || dto.YeniDurum.Value == SiparisDurumu.Beklemede)
        {
            throw new ArgumentException("Geçerli bir sipariş durumu seçin.");
        }

        var siparis = await _siparisDeposu.IdIleGetirAsync(dto.Id)
            ?? throw new KeyNotFoundException("Sipariş bulunamadı.");

        if (siparis.Durum == dto.YeniDurum.Value)
            return new SiparisDurumGuncelleSonucDto { Mesaj = "Sipariş zaten bu durumda. Yeni bildirim gönderilmedi." };

        siparis.Durum = dto.YeniDurum.Value;
        siparis.GuncellenmeTarihi = DateTime.UtcNow;

        _siparisDeposu.Guncelle(siparis);
        await _siparisDeposu.DegisiklikleriKaydetAsync();

        var mesaj = siparis.Durum switch
        {
            SiparisDurumu.Kargoda => $"{siparis.SiparisNumarasi} numaralı çiçek siparişiniz teslimat için yola çıktı.",
            SiparisDurumu.TeslimEdildi => $"{siparis.SiparisNumarasi} numaralı çiçek siparişiniz teslim edildi. Bizi tercih ettiğiniz için teşekkür ederiz.",
            SiparisDurumu.TeslimEdilemedi => $"{siparis.SiparisNumarasi} numaralı çiçek siparişiniz teslim edilemedi. Teslimatın yeniden planlanması için lütfen bizimle iletişime geçin.",
            _ => null
        };

        bool? smsKabulEdildi = mesaj == null ? null : await _smsServisi.SmsGonderAsync(siparis.MusteriTelefon, mesaj);

        var emailBilgisi = siparis.Durum switch
        {
            SiparisDurumu.Islemde => (Baslik: "Siparişiniz hazırlanmaya başladı", Aciklama: "Siparişiniz hazırlanıyor. Çiçekleriniz özenle hazırlanıp teslimata hazır hale getiriliyor."),
            SiparisDurumu.Kargoda => (Baslik: "Siparişiniz kargoya verildi", Aciklama: "Siparişiniz teslimat için yola çıktı. Teslimat bilgilerinizi sipariş detayından takip edebilirsiniz."),
            SiparisDurumu.TeslimEdildi => (Baslik: "Siparişiniz teslim edildi", Aciklama: "Siparişiniz başarıyla teslim edildi. Bizi tercih ettiğiniz için teşekkür ederiz."),
            _ => ((string Baslik, string Aciklama)?)null
        };

        bool? emailKabulEdildi = null;
        if (emailBilgisi.HasValue && !string.IsNullOrWhiteSpace(siparis.MusteriEmail))
        {
            var adSoyad = WebUtility.HtmlEncode($"{siparis.MusteriAd} {siparis.MusteriSoyad}".Trim());
            var siparisNo = WebUtility.HtmlEncode(siparis.SiparisNumarasi);
            var html = $"""
                <div style="font-family:Arial,sans-serif;max-width:620px;margin:0 auto;color:#26352d;line-height:1.6">
                    <div style="padding:24px 0;border-bottom:1px solid #e8eee9;color:#0d382b;font-size:22px;font-weight:700">ABC Çiçekçilik</div>
                    <div style="padding:28px 0">
                        <p style="margin:0 0 12px;font-size:16px">Merhaba {adSoyad},</p>
                        <h1 style="margin:0 0 14px;color:#0d382b;font-size:24px">{emailBilgisi.Value.Baslik}</h1>
                        <p style="margin:0 0 22px;color:#5f6d64">{emailBilgisi.Value.Aciklama}</p>
                        <div style="padding:16px 18px;background:#f7faf8;border:1px solid #dce9e1;border-radius:12px">
                            <strong>Sipariş numarası:</strong> {siparisNo}
                        </div>
                    </div>
                    <div style="padding:18px 0;border-top:1px solid #e8eee9;color:#718078;font-size:13px">Sorularınız için bizimle iletişime geçebilirsiniz.</div>
                </div>
                """;

            emailKabulEdildi = await _emailServisi.EpostaGonderAsync(
                siparis.MusteriEmail,
                $"ABC Çiçekçilik - {emailBilgisi.Value.Baslik} ({siparis.SiparisNumarasi})",
                html);
        }

        var kanalMesajlari = new List<string> { "Sipariş durumu güncellendi." };
        if (smsKabulEdildi == true) kanalMesajlari.Add("SMS gönderim isteği Netgsm tarafından kabul edildi.");
        else if (smsKabulEdildi == false) kanalMesajlari.Add("SMS gönderimi doğrulanamadı.");
        if (emailKabulEdildi == true) kanalMesajlari.Add("E-posta gönderildi.");
        else if (emailKabulEdildi == false) kanalMesajlari.Add("E-posta gönderimi başarısız oldu.");

        return new SiparisDurumGuncelleSonucDto
        {
            DurumDegisti = true,
            SmsKabulEdildi = smsKabulEdildi,
            EmailKabulEdildi = emailKabulEdildi,
            Mesaj = string.Join(" ", kanalMesajlari)
        };
    }
}
