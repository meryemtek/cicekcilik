using ECommerce.Application.DTOs.Siparis;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;

public class SiparisServisi : ISiparisServisi
{
    private readonly ISiparisDeposu _siparisDeposu; // Özel depo enjekte ediliyor
    private readonly ISepetServisi _sepetServisi;
    private readonly ISmsServisi _smsServisi;
    private readonly IHiCariEntegrasyonServisi _hiCariServisi;
    private readonly IDepo<Urun> _urunDeposu;
    private readonly IDepo<UrunVaryant> _varyantDeposu;

    public SiparisServisi(
        ISiparisDeposu siparisDeposu,
        ISepetServisi sepetServisi,
        ISmsServisi smsServisi,
        IHiCariEntegrasyonServisi hiCariServisi,
        IDepo<Urun> urunDeposu,
        IDepo<UrunVaryant> varyantDeposu)
    {
        _siparisDeposu = siparisDeposu;
        _sepetServisi = sepetServisi;
        _smsServisi = smsServisi;
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
        var sepet = await _sepetServisi.SepetGetirAsync(dto.SepetId);
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

        var siparisNo = "SP-" + DateTime.Now.ToString("yyyyMMdd") + "-" + new Random().Next(1000, 9999);

        var yeniSiparis = new Siparis
        {
            SiparisNumarasi = siparisNo,
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
        await _siparisDeposu.DegisiklikleriKaydetAsync();

        // 2. Stok Düşürme ve Stok 0 Olunca Pasife Çekme İşlemi
        foreach (var k in sepet.Kalemler)
        {
            var urun = await _urunDeposu.IdIleGetirAsync(k.UrunId);
            if (urun != null)
            {
                urun.StokMiktari -= k.Miktar;
                if (urun.StokMiktari <= 0)
                {
                    urun.StokMiktari = 0;
                    urun.AktifMi = false; // Stok 0 olunca ürün pasif duruma geçer
                }
                urun.GuncellenmeTarihi = DateTime.UtcNow;
                _urunDeposu.Guncelle(urun);
            }

            if (k.VaryantId.HasValue && k.VaryantId.Value != Guid.Empty)
            {
                var varyant = await _varyantDeposu.IdIleGetirAsync(k.VaryantId.Value);
                if (varyant != null)
                {
                    varyant.StokMiktari -= k.Miktar;
                    if (varyant.StokMiktari <= 0)
                    {
                        varyant.StokMiktari = 0;
                        varyant.AktifMi = false;
                    }
                    varyant.GuncellenmeTarihi = DateTime.UtcNow;
                    _varyantDeposu.Guncelle(varyant);
                }
            }
        }

        await _urunDeposu.DegisiklikleriKaydetAsync();
        await _varyantDeposu.DegisiklikleriKaydetAsync();

        await _sepetServisi.SepetiTemizleAsync(dto.SepetId);
        await _smsServisi.SmsGonderAsync(dto.MusteriTelefon, $"Sayin {dto.MusteriAd} {dto.MusteriSoyad}, {yeniSiparis.SiparisNumarasi} numarali siparisiniz basariyla alinmistir.");
        await _hiCariServisi.CariVeStokIsleAsync(yeniSiparis.Id);

        return yeniSiparis.Id;
    }

    public async Task DurumGuncelleAsync(SiparisDurumGuncelleDto dto)
    {
        var siparis = await _siparisDeposu.IdIleGetirAsync(dto.Id);
        if (siparis != null)
        {
            siparis.Durum = dto.YeniDurum;
            siparis.GuncellenmeTarihi = DateTime.UtcNow;

            _siparisDeposu.Guncelle(siparis);
            await _siparisDeposu.DegisiklikleriKaydetAsync();
        }
    }
}