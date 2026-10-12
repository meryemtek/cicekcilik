using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using ECommerce.Application.Models;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;

public class SepetServisi : ISepetServisi
{
    private readonly IDistributedCache _onbellek;
    private readonly IKuponServisi _kuponServisi;
    private readonly IKampanyaServisi _kampanyaServisi;
    private readonly IDepo<Urun> _urunDepo;
    private const int SepetGecerlilikGunSayisi = 7;

    public SepetServisi(IDistributedCache onbellek, IKuponServisi kuponServisi, IKampanyaServisi kampanyaServisi, IDepo<Urun> urunDepo)
    {
        _onbellek = onbellek;
        _kuponServisi = kuponServisi;
        _kampanyaServisi = kampanyaServisi;
        _urunDepo = urunDepo;
    }

    public async Task<Sepet> SepetGetirAsync(string sepetId)
    {
        var veri = await _onbellek.GetStringAsync(sepetId);
        if (string.IsNullOrEmpty(veri))
        {
            return new Sepet(sepetId);
        }

        var sepet = JsonSerializer.Deserialize<Sepet>(veri);
        if (sepet == null) return new Sepet(sepetId);

        // Kampanyaları hesaplayıp özete yansıtıyoruz
        await KampanyalariSepeteUygulaAsync(sepet);
        return sepet;
    }

    private async Task KampanyalariSepeteUygulaAsync(Sepet sepet)
    {
        sepet.KampanyaIndirimTutari = 0;
        sepet.UygulananKampanyaAdi = null;

        if (sepet.Kalemler == null || !sepet.Kalemler.Any()) return;

        var aktifKampanyalar = await _kampanyaServisi.GetAktifKampanyaUrunlerAsync();
        if (aktifKampanyalar == null || !aktifKampanyalar.Any()) return;

        var urunIds = sepet.Kalemler.Select(k => k.UrunId).Distinct().ToList();
        var urunListesi = await _urunDepo.GetirAsync(u => urunIds.Contains(u.Id));
        var urunKategoriSozlugu = urunListesi.ToDictionary(u => u.Id, u => u.KategoriId);

        decimal toplamKampanyaIndirimi = 0;
        string? kampanyaAdi = null;

        foreach (var kalem in sepet.Kalemler)
        {
            Guid? kategoriId = urunKategoriSozlugu.TryGetValue(kalem.UrunId, out var katId) ? katId : (Guid?)null;

            var uygunKampanya = aktifKampanyalar
                .Where(k =>
                    (k.KapsamTuru == "Urun" && k.UrunId == kalem.UrunId) ||
                    (k.KapsamTuru == "Kategori" && k.TetikleyiciKategoriId.HasValue && k.TetikleyiciKategoriId == kategoriId) ||
                    (k.KampanyaVaryasyonu == "KategoriCapraz" && k.TetikleyiciKategoriId.HasValue && k.TetikleyiciKategoriId == kategoriId)
                )
                .OrderBy(k => k.Sira)
                .FirstOrDefault();

            if (uygunKampanya != null)
            {
                decimal birimIndirim = 0;
                if (uygunKampanya.IndirimTipi == "Yuzde")
                {
                    birimIndirim = kalem.BirimFiyat * (uygunKampanya.IndirimDegeri / 100m);
                }
                else // Tutar bazlı
                {
                    birimIndirim = uygunKampanya.IndirimDegeri;
                }

                if (birimIndirim > kalem.BirimFiyat) birimIndirim = kalem.BirimFiyat;

                toplamKampanyaIndirimi += birimIndirim * kalem.Miktar;
                kampanyaAdi ??= uygunKampanya.KampanyaAdi;
            }
        }

        sepet.KampanyaIndirimTutari = toplamKampanyaIndirimi;
        sepet.UygulananKampanyaAdi = kampanyaAdi;
    }

    public async Task<Sepet> SepeteUrunEkleAsync(string sepetId, SepetKalemi kalem)
    {
        var sepet = await SepetGetirAsync(sepetId);

        var mevcutKalem = sepet.Kalemler.FirstOrDefault(x =>
            x.UrunId == kalem.UrunId &&
            x.VaryantId == kalem.VaryantId &&
            x.Il == kalem.Il &&
            x.Ilce == kalem.Ilce &&
            x.Mahalle == kalem.Mahalle &&
            x.TeslimatGunu == kalem.TeslimatGunu &&
            x.TeslimatSaati == kalem.TeslimatSaati &&
            x.VipTeslimatAktifMi == kalem.VipTeslimatAktifMi);

        if (mevcutKalem != null)
        {
            mevcutKalem.Miktar += kalem.Miktar;
        }
        else
        {
            sepet.Kalemler.Add(kalem);
        }

        await SepetiKaydetAsync(sepet);
        return sepet;
    }

    public async Task<Sepet> SepettenUrunCikarAsync(string sepetId, Guid kalemId)
    {
        var sepet = await SepetGetirAsync(sepetId);
        var mevcutKalem = sepet.Kalemler.FirstOrDefault(x => x.Id == kalemId);

        if (mevcutKalem != null)
        {
            sepet.Kalemler.Remove(mevcutKalem);
            await SepetiKaydetAsync(sepet);
        }

        return sepet;
    }

    public async Task<Sepet> UrunMiktariGuncelleAsync(string sepetId, Guid kalemId, int miktar)
    {
        var sepet = await SepetGetirAsync(sepetId);
        var mevcutKalem = sepet.Kalemler.FirstOrDefault(x => x.Id == kalemId);

        if (mevcutKalem != null)
        {
            if (miktar <= 0)
            {
                sepet.Kalemler.Remove(mevcutKalem);
            }
            else
            {
                mevcutKalem.Miktar = miktar;
            }

            await SepetiKaydetAsync(sepet);
        }

        return sepet;
    }

    public async Task<Sepet> UrunuAyirAsync(string sepetId, Guid kalemId)
    {
        var sepet = await SepetGetirAsync(sepetId);
        var hedefKalem = sepet.Kalemler.FirstOrDefault(x => x.Id == kalemId);

        if (hedefKalem != null)
        {
            if (hedefKalem.Miktar > 1)
            {
                hedefKalem.Miktar -= 1;
                var yeniKalem = JsonSerializer.Deserialize<SepetKalemi>(JsonSerializer.Serialize(hedefKalem));
                if (yeniKalem != null)
                {
                    yeniKalem.Id = Guid.NewGuid();
                    yeniKalem.Miktar = 1;
                    sepet.Kalemler.Add(yeniKalem);
                }
            }
            else
            {
                var yeniKalem = JsonSerializer.Deserialize<SepetKalemi>(JsonSerializer.Serialize(hedefKalem));
                if (yeniKalem != null)
                {
                    yeniKalem.Id = Guid.NewGuid();
                    yeniKalem.Miktar = 1;
                    sepet.Kalemler.Add(yeniKalem);
                }
            }

            await SepetiKaydetAsync(sepet);
        }

        return sepet;
    }

    public async Task<Sepet> SepetUrunleriniBirlestirAsync(string sepetId)
    {
        var sepet = await SepetGetirAsync(sepetId);
        if (sepet.Kalemler == null || !sepet.Kalemler.Any()) return sepet;

        var birlesmisKalemler = new List<SepetKalemi>();

        foreach (var kalem in sepet.Kalemler)
        {
            var mevcut = birlesmisKalemler.FirstOrDefault(x =>
                x.UrunId == kalem.UrunId &&
                x.VaryantId == kalem.VaryantId &&
                x.Il == kalem.Il &&
                x.Ilce == kalem.Ilce &&
                x.Mahalle == kalem.Mahalle &&
                x.TeslimatGunu == kalem.TeslimatGunu &&
                x.TeslimatSaati == kalem.TeslimatSaati &&
                x.VipTeslimatAktifMi == kalem.VipTeslimatAktifMi);

            if (mevcut != null)
            {
                mevcut.Miktar += kalem.Miktar;
            }
            else
            {
                birlesmisKalemler.Add(new SepetKalemi
                {
                    Id = Guid.NewGuid(),
                    UrunId = kalem.UrunId,
                    UrunAdi = kalem.UrunAdi,
                    VaryantId = kalem.VaryantId,
                    VaryantAdi = kalem.VaryantAdi,
                    BirimFiyat = kalem.BirimFiyat,
                    Miktar = kalem.Miktar,
                    GorselUrl = kalem.GorselUrl,
                    Il = kalem.Il,
                    Ilce = kalem.Ilce,
                    Mahalle = kalem.Mahalle,
                    TeslimatGunu = kalem.TeslimatGunu,
                    TeslimatSaati = kalem.TeslimatSaati,
                    VipTeslimatAktifMi = kalem.VipTeslimatAktifMi,
                    TeslimatUcreti = kalem.TeslimatUcreti,
                    VipUcreti = kalem.VipUcreti
                });
            }
        }

        sepet.Kalemler = birlesmisKalemler;
        await SepetiKaydetAsync(sepet);
        return sepet;
    }

    public async Task<Sepet> KuponUygulaAsync(string sepetId, string kuponKodu)
    {
        var sepet = await SepetGetirAsync(sepetId);
        if (string.IsNullOrWhiteSpace(kuponKodu))
            throw new Exception("Lütfen geçerli bir indirim kodu giriniz.");

        var aktifKampanyalar = await _kampanyaServisi.GetAktifKampanyaUrunlerAsync();
        var urunIds = sepet.Kalemler.Select(k => k.UrunId).Distinct().ToList();
        var urunListesi = await _urunDepo.GetirAsync(u => urunIds.Contains(u.Id));
        var urunKategoriSozlugu = urunListesi.ToDictionary(u => u.Id, u => u.KategoriId);

        foreach (var kalem in sepet.Kalemler)
        {
            Guid? kategoriId = urunKategoriSozlugu.TryGetValue(kalem.UrunId, out var katId) ? katId : (Guid?)null;

            var kampanya = aktifKampanyalar.FirstOrDefault(k =>
                (k.KapsamTuru == "Urun" && k.UrunId == kalem.UrunId) ||
                (k.KapsamTuru == "Kategori" && k.TetikleyiciKategoriId.HasValue && k.TetikleyiciKategoriId == kategoriId) ||
                (k.KampanyaVaryasyonu == "KategoriCapraz" && k.TetikleyiciKategoriId.HasValue && k.TetikleyiciKategoriId == kategoriId)
            );

            if (kampanya != null && !kampanya.KuponlarlaBirlesebilirMi)
            {
                throw new Exception("Sepetinizde kuponla birleştirilemeyen kampanyalı ürün bulunmaktadır.");
            }
        }

        var kupon = await _kuponServisi.KodIleGetirAsync(kuponKodu);
        if (kupon == null)
            throw new Exception("Böyle bir indirim kuponu bulunamadı.");

        if (!kupon.AktifMi)
            throw new Exception("Bu indirim kuponu aktif değil.");

        if (kupon.BitisTarihi < DateTime.Now)
            throw new Exception("Bu indirim kuponunun süresi dolmuş.");

        decimal araToplam = sepet.Kalemler.Sum(k => k.BirimFiyat * k.Miktar);

        if (kupon.YuzdelikMi)
        {
            sepet.IndirimKodu = kupon.Kod;
            sepet.IndirimTutari = araToplam * (kupon.IndirimMiktari / 100m);
        }
        else
        {
            sepet.IndirimKodu = kupon.Kod;
            sepet.IndirimTutari = kupon.IndirimMiktari;
        }

        if (sepet.IndirimTutari > araToplam)
        {
            sepet.IndirimTutari = araToplam;
        }

        await SepetiKaydetAsync(sepet);
        return sepet;
    }

    public async Task<Sepet> KuponKaldirAsync(string sepetId)
    {
        var sepet = await SepetGetirAsync(sepetId);
        sepet.IndirimKodu = null;
        sepet.IndirimTutari = 0;
        await SepetiKaydetAsync(sepet);
        return sepet;
    }

    public async Task SepetiTemizleAsync(string sepetId)
    {
        await _onbellek.RemoveAsync(sepetId);
    }

    private async Task SepetiKaydetAsync(Sepet sepet)
    {
        var ayarlar = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(SepetGecerlilikGunSayisi)
        };

        var serilesmisSepet = JsonSerializer.Serialize(sepet);
        await _onbellek.SetStringAsync(sepet.SepetId, serilesmisSepet, ayarlar);
    }
}