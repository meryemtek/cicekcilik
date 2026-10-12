using ECommerce.Application.DTOs.Urun;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services;

public class TopluUrunYonetimServisi : ITopluUrunYonetimServisi
{
    private readonly IDepo<Urun> _urunDeposu;

    public TopluUrunYonetimServisi(IDepo<Urun> urunDeposu)
    {
        _urunDeposu = urunDeposu;
    }

    public async Task TopluFiyatGuncelleAsync(TopluFiyatGuncelleDto dto)
    {
        // Kategori filtresi verilmişse o kategoriye ait, verilmemişse tüm ürünleri getir
        IReadOnlyList<Urun> urunler;
        if (dto.KategoriId.HasValue)
        {
            urunler = await _urunDeposu.GetirAsync(u => u.KategoriId == dto.KategoriId.Value);
        }
        else
        {
            urunler = await _urunDeposu.TumunuGetirAsync();
        }

        foreach (var urun in urunler)
        {
            decimal degisimMiktari = 0;

            if (dto.YuzdelikMi)
            {
                degisimMiktari = urun.Fiyat * (dto.OranVeyaMiktar / 100m);
            }
            else
            {
                degisimMiktari = dto.OranVeyaMiktar;
            }

            if (dto.ArtisMi)
            {
                urun.Fiyat += degisimMiktari;
            }
            else
            {
                urun.Fiyat -= degisimMiktari;
                if (urun.Fiyat < 0) urun.Fiyat = 0; // Fiyat eksiye düşemez
            }

            urun.GuncellenmeTarihi = DateTime.UtcNow;
            _urunDeposu.Guncelle(urun);
        }

        await _urunDeposu.DegisiklikleriKaydetAsync();
    }

    public async Task TopluDurumDegistirAsync(bool aktifMi, Guid? kategoriId = null)
    {
        IReadOnlyList<Urun> urunler;
        if (kategoriId.HasValue)
        {
            urunler = await _urunDeposu.GetirAsync(u => u.KategoriId == kategoriId.Value);
        }
        else
        {
            urunler = await _urunDeposu.TumunuGetirAsync();
        }

        foreach (var urun in urunler)
        {
            urun.AktifMi = aktifMi;
            urun.GuncellenmeTarihi = DateTime.UtcNow;
            _urunDeposu.Guncelle(urun);
        }

        await _urunDeposu.DegisiklikleriKaydetAsync();
    }
}