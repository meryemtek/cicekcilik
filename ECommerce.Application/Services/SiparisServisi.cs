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

    public SiparisServisi(
        ISiparisDeposu siparisDeposu,
        ISepetServisi sepetServisi,
        ISmsServisi smsServisi,
        IHiCariEntegrasyonServisi hiCariServisi)
    {
        _siparisDeposu = siparisDeposu;
        _sepetServisi = sepetServisi;
        _smsServisi = smsServisi;
        _hiCariServisi = hiCariServisi;
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