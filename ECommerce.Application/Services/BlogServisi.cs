using ECommerce.Application.DTOs.Blog;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services;

public class BlogServisi : IBlogServisi
{
    private readonly IDepo<BlogYazisi> _blogDeposu;

    public BlogServisi(IDepo<BlogYazisi> blogDeposu)
    {
        _blogDeposu = blogDeposu;
    }

    public async Task<IReadOnlyList<BlogListeDto>> TumYazilariGetirAsync()
    {
        var yazilar = await _blogDeposu.TumunuGetirAsync();

        return yazilar.Select(y => new BlogListeDto
        {
            Id = y.Id,
            Baslik = y.Baslik,
            SeoUrl = y.SeoUrl,
            Ozet = y.Ozet,
            GorselUrl = y.GorselUrl,
            OkunmaSayisi = y.OkunmaSayisi,
            AktifMi = y.AktifMi,
            YayinlanmaTarihi = y.YayinlanmaTarihi
        })
        .OrderByDescending(y => y.YayinlanmaTarihi) // En yeni yazılar en üstte gelsin diye tarihe göre sıraladık
        .ToList();
    }

    public async Task<BlogGuncelleDto?> IdIleGetirAsync(Guid id)
    {
        var yazi = await _blogDeposu.IdIleGetirAsync(id);
        if (yazi == null) return null;

        return new BlogGuncelleDto
        {
            Id = yazi.Id,
            Baslik = yazi.Baslik,
            SeoUrl = yazi.SeoUrl,
            MetaBaslik = yazi.MetaBaslik,
            MetaAciklama = yazi.MetaAciklama,
            Ozet = yazi.Ozet,
            Icerik = yazi.Icerik,
            GorselUrl = yazi.GorselUrl,
            AktifMi = yazi.AktifMi,
            YayinlanmaTarihi = yazi.YayinlanmaTarihi // Eklendi
        };
    }

    // YENİ EKLENEN METOT: Ön yüz detay sayfası için
    public async Task<BlogGuncelleDto?> SeoUrlIleGetirAsync(string seoUrl)
    {
        var yazilar = await _blogDeposu.TumunuGetirAsync();
        var yazi = yazilar.FirstOrDefault(x => x.SeoUrl == seoUrl && x.AktifMi);

        if (yazi == null) return null;

        // Yazı her görüntülendiğinde okunma sayısını artırıyoruz
        yazi.OkunmaSayisi += 1;
        _blogDeposu.Guncelle(yazi);
        await _blogDeposu.DegisiklikleriKaydetAsync();

        return new BlogGuncelleDto
        {
            Id = yazi.Id,
            Baslik = yazi.Baslik,
            SeoUrl = yazi.SeoUrl,
            MetaBaslik = yazi.MetaBaslik,
            MetaAciklama = yazi.MetaAciklama,
            Ozet = yazi.Ozet,
            Icerik = yazi.Icerik,
            GorselUrl = yazi.GorselUrl,
            AktifMi = yazi.AktifMi,
            YayinlanmaTarihi = yazi.YayinlanmaTarihi
        };
    }

    public async Task<Guid> EkleAsync(BlogEkleDto dto)
    {
        var yeniYazi = new BlogYazisi
        {
            Baslik = dto.Baslik,
            SeoUrl = dto.SeoUrl,
            MetaBaslik = dto.MetaBaslik,
            MetaAciklama = dto.MetaAciklama,
            Ozet = dto.Ozet,
            Icerik = dto.Icerik,
            GorselUrl = dto.GorselUrl,
            OkunmaSayisi = 0,
            AktifMi = dto.AktifMi,
            YayinlanmaTarihi = dto.YayinlanmaTarihi // DTO'dan alınıyor (İleri tarihli yayınlanma desteği için)
        };

        await _blogDeposu.EkleAsync(yeniYazi);
        await _blogDeposu.DegisiklikleriKaydetAsync();

        return yeniYazi.Id;
    }

    public async Task GuncelleAsync(BlogGuncelleDto dto)
    {
        var yazi = await _blogDeposu.IdIleGetirAsync(dto.Id);
        if (yazi != null)
        {
            yazi.Baslik = dto.Baslik;
            yazi.SeoUrl = dto.SeoUrl;
            yazi.MetaBaslik = dto.MetaBaslik;
            yazi.MetaAciklama = dto.MetaAciklama;
            yazi.Ozet = dto.Ozet;
            yazi.Icerik = dto.Icerik;
            yazi.GorselUrl = dto.GorselUrl;
            yazi.AktifMi = dto.AktifMi;
            yazi.YayinlanmaTarihi = dto.YayinlanmaTarihi; // Eklendi
            yazi.GuncellenmeTarihi = DateTime.UtcNow;

            _blogDeposu.Guncelle(yazi);
            await _blogDeposu.DegisiklikleriKaydetAsync();
        }
    }

    public async Task SilAsync(Guid id)
    {
        var yazi = await _blogDeposu.IdIleGetirAsync(id);
        if (yazi != null)
        {
            _blogDeposu.Sil(yazi);
            await _blogDeposu.DegisiklikleriKaydetAsync();
        }
    }
}