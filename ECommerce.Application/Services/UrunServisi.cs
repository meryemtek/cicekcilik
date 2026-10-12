using ECommerce.Application.DTOs.Urun;
using ECommerce.Application.DTOs.UrunVaryant;
using ECommerce.Application.DTOs.UrunYorum;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services;

public class UrunServisi : IUrunServisi
{
    private readonly IDepo<Urun> _urunDeposu;
    private readonly IDepo<UrunVaryant> _varyantDeposu;
    private readonly IDepo<UrunYorum> _yorumDeposu;
    private readonly IDepo<Kategori> _kategoriDeposu;

    public UrunServisi(
        IDepo<Urun> urunDeposu,
        IDepo<UrunVaryant> varyantDeposu,
        IDepo<UrunYorum> yorumDeposu,
        IDepo<Kategori> kategoriDeposu)
    {
        _urunDeposu = urunDeposu;
        _varyantDeposu = varyantDeposu;
        _yorumDeposu = yorumDeposu;
        _kategoriDeposu = kategoriDeposu;
    }

    public async Task<IReadOnlyList<UrunListeDto>> TumUrunleriGetirAsync()
    {
        var urunler = await _urunDeposu.TumunuGetirAsync();

        return urunler.Select(u => new UrunListeDto
        {
            Id = u.Id,
            Ad = u.Ad,
            SeoUrl = u.SeoUrl,
            UrunKodu = u.UrunKodu,
            Fiyat = u.Fiyat,
            StokMiktari = u.StokMiktari,
            AktifMi = u.AktifMi,
            KategoriId = u.KategoriId,
            Etiket = u.Etiket,
            GorselUrl = u.GorselUrl
        }).ToList();
    }

    public async Task<UrunGuncelleDto?> IdIleGetirAsync(Guid id)
    {
        var urun = await _urunDeposu.IdIleGetirAsync(id);
        if (urun == null) return null;

        return new UrunGuncelleDto
        {
            Id = urun.Id,
            Ad = urun.Ad,
            UrunKodu = urun.UrunKodu,
            SeoUrl = urun.SeoUrl,
            Aciklama = urun.Aciklama,
            AiAciklama = urun.AiAciklama,
            Fiyat = urun.Fiyat,
            StokMiktari = urun.StokMiktari,
            GorselUrl = urun.GorselUrl,
            KategoriId = urun.KategoriId,
            Etiket = urun.Etiket,
            AktifMi = urun.AktifMi
        };
    }

    public async Task<Guid> EkleAsync(UrunEkleDto dto)
    {
        var yeniUrun = new Urun
        {
            Ad = dto.Ad,
            UrunKodu = dto.UrunKodu,
            SeoUrl = dto.SeoUrl,
            Aciklama = dto.Aciklama,
            AiAciklama = dto.AiAciklama,
            Fiyat = dto.Fiyat,
            StokMiktari = dto.StokMiktari,
            GorselUrl = dto.GorselUrl,
            KategoriId = dto.KategoriId,
            Etiket = dto.Etiket,
            AktifMi = dto.AktifMi
        };

        await _urunDeposu.EkleAsync(yeniUrun);
        await _urunDeposu.DegisiklikleriKaydetAsync();

        return yeniUrun.Id;
    }

    public async Task GuncelleAsync(UrunGuncelleDto dto)
    {
        var urun = await _urunDeposu.IdIleGetirAsync(dto.Id);
        if (urun != null)
        {
            urun.Ad = dto.Ad;
            urun.UrunKodu = dto.UrunKodu;
            urun.SeoUrl = dto.SeoUrl;
            urun.Aciklama = dto.Aciklama;
            urun.Fiyat = dto.Fiyat;
            urun.StokMiktari = dto.StokMiktari;
            urun.AktifMi = dto.AktifMi;
            urun.KategoriId = dto.KategoriId;
            urun.Etiket = dto.Etiket;

            if (!string.IsNullOrWhiteSpace(dto.GorselUrl))
            {
                urun.GorselUrl = dto.GorselUrl;
            }

            urun.GuncellenmeTarihi = DateTime.UtcNow;

            _urunDeposu.Guncelle(urun);
            await _urunDeposu.DegisiklikleriKaydetAsync();
        }
    }

    public async Task DurumDegistirAsync(Guid id, bool aktifMi)
    {
        var urun = await _urunDeposu.IdIleGetirAsync(id);
        if (urun != null)
        {
            urun.AktifMi = aktifMi;
            urun.GuncellenmeTarihi = DateTime.UtcNow;
            _urunDeposu.Guncelle(urun);
            await _urunDeposu.DegisiklikleriKaydetAsync();
        }
    }

    public async Task SilAsync(Guid id)
    {
        var urun = await _urunDeposu.IdIleGetirAsync(id);
        if (urun != null)
        {
            _urunDeposu.Sil(urun);
            await _urunDeposu.DegisiklikleriKaydetAsync();
        }
    }

    public async Task<List<UrunListeDto>> KategoriIdyeGoreGetirAsync(Guid kategoriId)
    {
        var tumUrunler = await _urunDeposu.TumunuGetirAsync();

        var urunler = tumUrunler
            .Where(x => x.KategoriId == kategoriId && x.AktifMi)
            .Select(x => new UrunListeDto
            {
                Id = x.Id,
                Ad = x.Ad,
                SeoUrl = x.SeoUrl,
                UrunKodu = x.UrunKodu,
                Fiyat = x.Fiyat,
                StokMiktari = x.StokMiktari,
                AktifMi = x.AktifMi,
                KategoriId = x.KategoriId,
                Etiket = x.Etiket,
                GorselUrl = x.GorselUrl
            })
            .ToList();

        return urunler;
    }

    public async Task<UrunDetayDto?> SeoUrlIleDetayGetirAsync(string seoUrl)
    {
        var urunler = await _urunDeposu.GetirAsync(u => u.SeoUrl == seoUrl && u.AktifMi);
        var tekUrun = urunler.FirstOrDefault();
        if (tekUrun == null) return null;

        var kategori = await _kategoriDeposu.IdIleGetirAsync(tekUrun.KategoriId);
        var varyantlar = await _varyantDeposu.GetirAsync(v => v.UrunId == tekUrun.Id && v.AktifMi);
        var yorumlar = await _yorumDeposu.GetirAsync(y => y.UrunId == tekUrun.Id);

        var onayliYorumlar = yorumlar.Where(y => y.OnaylandiMi).OrderByDescending(y => y.OlusturulmaTarihi).ToList();
        double ortalamaPuan = onayliYorumlar.Any() ? onayliYorumlar.Average(y => y.Puan) : 0;

        return new UrunDetayDto
        {
            Id = tekUrun.Id,
            Ad = tekUrun.Ad,
            UrunKodu = tekUrun.UrunKodu,
            SeoUrl = tekUrun.SeoUrl,
            Aciklama = tekUrun.Aciklama,
            AiAciklama = tekUrun.AiAciklama,
            Fiyat = tekUrun.Fiyat,
            StokMiktari = tekUrun.StokMiktari,
            GorselUrl = tekUrun.GorselUrl,
            Etiket = tekUrun.Etiket,
            KategoriId = tekUrun.KategoriId,
            KategoriAdi = kategori?.Ad ?? string.Empty,

            Varyantlar = varyantlar.Select(v => new UrunVaryantListeDto
            {
                Id = v.Id,
                UrunId = v.UrunId,
                VaryantAdi = v.VaryantAdi,
                FiyatFarki = v.FiyatFarki,
                StokMiktari = v.StokMiktari,
                AktifMi = v.AktifMi
            }).ToList(),

            Yorumlar = onayliYorumlar.Select(y => new UrunYorumListeDto
            {
                Id = y.Id,
                UrunId = y.UrunId,
                YorumcuAd = y.YorumcuAd,
                Puan = y.Puan,
                OnaylandiMi = y.OnaylandiMi,
                OlusturulmaTarihi = y.OlusturulmaTarihi
            }).ToList(),

            OrtalamaPuan = Math.Round(ortalamaPuan, 1),
            ToplamYorumSayisi = onayliYorumlar.Count()
        };
    }
    public async Task<UrunDetayDto?> IdIleDetayGetirAsync(Guid id)
    {
        var tekUrun = await _urunDeposu.IdIleGetirAsync(id);
        if (tekUrun == null) return null;

        var kategori = await _kategoriDeposu.IdIleGetirAsync(tekUrun.KategoriId);
        var varyantlar = await _varyantDeposu.GetirAsync(v => v.UrunId == tekUrun.Id && v.AktifMi);
        var yorumlar = await _yorumDeposu.GetirAsync(y => y.UrunId == tekUrun.Id);

        var onayliYorumlar = yorumlar.Where(y => y.OnaylandiMi).ToList();
        double ortalamaPuan = onayliYorumlar.Any() ? onayliYorumlar.Average(y => y.Puan) : 0;

        return new UrunDetayDto
        {
            Id = tekUrun.Id,
            Ad = tekUrun.Ad,
            UrunKodu = tekUrun.UrunKodu,
            SeoUrl = tekUrun.SeoUrl,
            Aciklama = tekUrun.Aciklama,
            AiAciklama = tekUrun.AiAciklama,
            Fiyat = tekUrun.Fiyat,
            StokMiktari = tekUrun.StokMiktari,
            GorselUrl = tekUrun.GorselUrl,
            Etiket = tekUrun.Etiket,
            KategoriId = tekUrun.KategoriId,
            KategoriAdi = kategori?.Ad ?? string.Empty,

            // VARYANTLAR ARTIK SEPETE DOĞRU AKTARILACAK
            Varyantlar = varyantlar.Select(v => new UrunVaryantListeDto
            {
                Id = v.Id,
                UrunId = v.UrunId,
                VaryantAdi = v.VaryantAdi,
                FiyatFarki = v.FiyatFarki,
                StokMiktari = v.StokMiktari,
                AktifMi = v.AktifMi
            }).ToList(),

            OrtalamaPuan = Math.Round(ortalamaPuan, 1),
            ToplamYorumSayisi = onayliYorumlar.Count()
        };
    }

    public async Task UrunYorumuEkleAsync(UrunYorumEkleDto dto)
    {
        var yorum = new UrunYorum
        {
            UrunId = dto.UrunId,
            YorumcuAd = dto.YorumcuAd,
            YorumMetni = dto.YorumMetni,
            Puan = dto.Puan,
            OnaylandiMi = false
        };

        await _yorumDeposu.EkleAsync(yorum);
        await _yorumDeposu.DegisiklikleriKaydetAsync();
    }
}