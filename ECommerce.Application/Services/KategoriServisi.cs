using ECommerce.Application.DTOs.Kategori;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Application.Services;

public class KategoriServisi : IKategoriServisi
{
    private readonly IDepo<Kategori> _kategoriDeposu;

    public KategoriServisi(IDepo<Kategori> kategoriDeposu)
    {
        _kategoriDeposu = kategoriDeposu;
    }

    public async Task<IReadOnlyList<KategoriListeDto>> TumKategorileriGetirAsync()
    {
        var kategoriler = await _kategoriDeposu.TumunuGetirAsync();

        return kategoriler.Select(k => new KategoriListeDto
        {
            Id = k.Id,
            Ad = k.Ad,
            SeoUrl = k.SeoUrl,
            GorselUrl = k.GorselUrl,
            AktifMi = k.AktifMi,
            UstKategoriId = k.UstKategoriId,
            UstKategoriAdi = kategoriler.FirstOrDefault(x => x.Id == k.UstKategoriId)?.Ad
        }).ToList();
    }

    public async Task<KategoriGuncelleDto?> IdIleGetirAsync(Guid id)
    {
        var kategori = await _kategoriDeposu.IdIleGetirAsync(id);
        if (kategori == null) return null;

        return new KategoriGuncelleDto
        {
            Id = kategori.Id,
            Ad = kategori.Ad,
            SeoUrl = kategori.SeoUrl,
            Aciklama = kategori.Aciklama,
            GorselUrl = kategori.GorselUrl,
            AktifMi = kategori.AktifMi,
            UstKategoriId = kategori.UstKategoriId
        };
    }

    public async Task<Guid> EkleAsync(KategoriEkleDto dto)
    {
        var yeniKategori = new Kategori
        {
            Ad = dto.Ad,
            SeoUrl = dto.SeoUrl,
            Aciklama = dto.Aciklama ?? string.Empty,
            GorselUrl = dto.GorselUrl ?? string.Empty,
            AktifMi = dto.AktifMi,
            UstKategoriId = dto.UstKategoriId
        };

        await _kategoriDeposu.EkleAsync(yeniKategori);
        await _kategoriDeposu.DegisiklikleriKaydetAsync();

        return yeniKategori.Id;
    }

    public async Task GuncelleAsync(KategoriGuncelleDto dto)
    {
        var kategori = await _kategoriDeposu.IdIleGetirAsync(dto.Id);
        if (kategori != null)
        {
            kategori.Ad = dto.Ad;
            kategori.SeoUrl = dto.SeoUrl;
            kategori.Aciklama = dto.Aciklama ?? string.Empty;
            kategori.GorselUrl = dto.GorselUrl ?? string.Empty;
            kategori.AktifMi = dto.AktifMi;
            kategori.UstKategoriId = dto.UstKategoriId;
            kategori.GuncellenmeTarihi = DateTime.UtcNow;

            _kategoriDeposu.Guncelle(kategori);
            await _kategoriDeposu.DegisiklikleriKaydetAsync();
        }
    }

    public async Task SilAsync(Guid id)
    {
        var kategori = await _kategoriDeposu.IdIleGetirAsync(id);
        if (kategori != null)
        {
            _kategoriDeposu.Sil(kategori);
            await _kategoriDeposu.DegisiklikleriKaydetAsync();
        }
    }
}