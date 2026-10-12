using ECommerce.Application.DTOs.Sayfa;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services;

public class SayfaServisi : ISayfaServisi
{
    private readonly IDepo<Sayfa> _sayfaDeposu;

    public SayfaServisi(IDepo<Sayfa> sayfaDeposu)
    {
        _sayfaDeposu = sayfaDeposu;
    }

    public async Task<IReadOnlyList<SayfaListeDto>> TumSayfalariGetirAsync()
    {
        var sayfalar = await _sayfaDeposu.TumunuGetirAsync();

        return sayfalar.Select(s => new SayfaListeDto
        {
            Id = s.Id,
            Baslik = s.Baslik,
            SayfaKodu = s.SayfaKodu,
            SeoUrl = s.SeoUrl,
            AktifMi = s.AktifMi
        }).ToList();
    }

    public async Task<SayfaGuncelleDto?> IdIleGetirAsync(Guid id)
    {
        var sayfa = await _sayfaDeposu.IdIleGetirAsync(id);
        if (sayfa == null) return null;

        return new SayfaGuncelleDto
        {
            Id = sayfa.Id,
            Baslik = sayfa.Baslik,
            SayfaKodu = sayfa.SayfaKodu,
            SeoUrl = sayfa.SeoUrl,
            Icerik = sayfa.Icerik,
            AktifMi = sayfa.AktifMi
        };
    }

    public async Task<SayfaGuncelleDto?> KodIleGetirAsync(string sayfaKodu)
    {
        var sayfalar = await _sayfaDeposu.TumunuGetirAsync();
        if (string.IsNullOrEmpty(sayfaKodu)) return null;

        var trCulture = new System.Globalization.CultureInfo("tr-TR");

        // Aranan kodu ve veritabanındaki kodları Türkçe kültür ile normalize ediyoruz
        string aranan = sayfaKodu.Trim()
            .ToLower(trCulture)
            .Replace("-", "_")
            .Replace("İ", "i").Replace("I", "i").Replace("ı", "i")
            .Replace("Ş", "s").Replace("ş", "s")
            .Replace("Ğ", "g").Replace("ğ", "g")
            .Replace("Ü", "u").Replace("ü", "u")
            .Replace("Ö", "o").Replace("ö", "o")
            .Replace("Ç", "c").Replace("ç", "c");

        var sayfa = sayfalar.FirstOrDefault(s =>
        {
            if (!s.AktifMi || string.IsNullOrEmpty(s.SayfaKodu)) return false;

            string dbKod = s.SayfaKodu.Trim()
                .ToLower(trCulture)
                .Replace("-", "_")
                .Replace("İ", "i").Replace("I", "i").Replace("ı", "i")
                .Replace("Ş", "s").Replace("ş", "s")
                .Replace("Ğ", "g").Replace("ğ", "g")
                .Replace("Ü", "u").Replace("ü", "u")
                .Replace("Ö", "o").Replace("ö", "o")
                .Replace("Ç", "c").Replace("ç", "c");

            return dbKod == aranan || s.SayfaKodu.Equals(sayfaKodu, StringComparison.OrdinalIgnoreCase);
        });

        if (sayfa == null) return null;

        return new SayfaGuncelleDto
        {
            Id = sayfa.Id,
            Baslik = sayfa.Baslik,
            SayfaKodu = sayfa.SayfaKodu,
            SeoUrl = sayfa.SeoUrl,
            Icerik = sayfa.Icerik,
            AktifMi = sayfa.AktifMi
        };
    }

    public async Task<Guid> EkleAsync(SayfaEkleDto dto)
    {
        var yeniSayfa = new Sayfa
        {
            Baslik = dto.Baslik,
            SayfaKodu = dto.SayfaKodu,
            SeoUrl = dto.SeoUrl,
            Icerik = dto.Icerik,
            MetaDescription = dto.MetaDescription,
            MetaKeywords = dto.MetaKeywords,
            AktifMi = dto.AktifMi
        };

        await _sayfaDeposu.EkleAsync(yeniSayfa);
        await _sayfaDeposu.DegisiklikleriKaydetAsync();

        return yeniSayfa.Id;
    }

    public async Task GuncelleAsync(SayfaGuncelleDto dto)
    {
        var sayfa = await _sayfaDeposu.IdIleGetirAsync(dto.Id);
        if (sayfa != null)
        {
            sayfa.Baslik = dto.Baslik;
            sayfa.SayfaKodu = dto.SayfaKodu;
            sayfa.SeoUrl = dto.SeoUrl;
            sayfa.Icerik = dto.Icerik;
            sayfa.AktifMi = dto.AktifMi;
            sayfa.MetaDescription = dto.MetaDescription;
            sayfa.MetaKeywords = dto.MetaKeywords;
            sayfa.GuncellenmeTarihi = DateTime.UtcNow;

            _sayfaDeposu.Guncelle(sayfa);
            await _sayfaDeposu.DegisiklikleriKaydetAsync();
        }
    }

    public async Task SilAsync(Guid id)
    {
        var sayfa = await _sayfaDeposu.IdIleGetirAsync(id);
        if (sayfa != null)
        {
            _sayfaDeposu.Sil(sayfa);
            await _sayfaDeposu.DegisiklikleriKaydetAsync();
        }
    }
}