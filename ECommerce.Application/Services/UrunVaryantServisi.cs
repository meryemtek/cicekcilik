using ECommerce.Application.DTOs.UrunVaryant;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services;

public class UrunVaryantServisi : IUrunVaryantServisi
{
    private readonly IDepo<UrunVaryant> _varyantDeposu;

    public UrunVaryantServisi(IDepo<UrunVaryant> varyantDeposu)
    {
        _varyantDeposu = varyantDeposu;
    }

    public async Task<IReadOnlyList<UrunVaryantListeDto>> UrununVaryantlariniGetirAsync(Guid urunId)
    {
        var varyantlar = await _varyantDeposu.GetirAsync(v => v.UrunId == urunId);

        return varyantlar.Select(v => new UrunVaryantListeDto
        {
            Id = v.Id,
            UrunId = v.UrunId,
            VaryantAdi = v.VaryantAdi,
            FiyatFarki = v.FiyatFarki,
            StokMiktari = v.StokMiktari,
            AktifMi = v.AktifMi
        }).ToList();
    }

    public async Task<UrunVaryantGuncelleDto?> IdIleGetirAsync(Guid id)
    {
        var varyant = await _varyantDeposu.IdIleGetirAsync(id);
        if (varyant == null) return null;

        return new UrunVaryantGuncelleDto
        {
            Id = varyant.Id,
            UrunId = varyant.UrunId,
            VaryantAdi = varyant.VaryantAdi,
            FiyatFarki = varyant.FiyatFarki,
            StokMiktari = varyant.StokMiktari,
            AktifMi = varyant.AktifMi
        };
    }

    public async Task<Guid> EkleAsync(UrunVaryantEkleDto dto)
    {
        var yeniVaryant = new UrunVaryant
        {
            UrunId = dto.UrunId,
            VaryantAdi = dto.VaryantAdi,
            FiyatFarki = dto.FiyatFarki,
            StokMiktari = dto.StokMiktari,
            AktifMi = dto.AktifMi
        };

        await _varyantDeposu.EkleAsync(yeniVaryant);
        await _varyantDeposu.DegisiklikleriKaydetAsync();

        return yeniVaryant.Id;
    }

    public async Task GuncelleAsync(UrunVaryantGuncelleDto dto)
    {
        var varyant = await _varyantDeposu.IdIleGetirAsync(dto.Id);
        if (varyant != null)
        {
            varyant.VaryantAdi = dto.VaryantAdi;
            varyant.FiyatFarki = dto.FiyatFarki;
            varyant.StokMiktari = dto.StokMiktari;
            varyant.AktifMi = dto.AktifMi;
            varyant.GuncellenmeTarihi = DateTime.UtcNow;

            _varyantDeposu.Guncelle(varyant);
            await _varyantDeposu.DegisiklikleriKaydetAsync();
        }
    }

    public async Task SilAsync(Guid id)
    {
        var varyant = await _varyantDeposu.IdIleGetirAsync(id);
        if (varyant != null)
        {
            _varyantDeposu.Sil(varyant);
            await _varyantDeposu.DegisiklikleriKaydetAsync();
        }
    }
}