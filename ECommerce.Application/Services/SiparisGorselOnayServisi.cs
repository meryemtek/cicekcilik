using ECommerce.Application.DTOs.SiparisGorselOnay;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services;

public class SiparisGorselOnayServisi : ISiparisGorselOnayServisi
{
    private readonly IDepo<SiparisGorselOnay> _gorselDeposu;

    public SiparisGorselOnayServisi(IDepo<SiparisGorselOnay> gorselDeposu)
    {
        _gorselDeposu = gorselDeposu;
    }

    public async Task<SiparisGorselOnayDto?> SiparisGorselGetirAsync(Guid siparisId)
    {
        var liste = await _gorselDeposu.GetirAsync(g => g.SiparisId == siparisId);
        var gorsel = liste.FirstOrDefault();

        if (gorsel == null) return null;

        return new SiparisGorselOnayDto
        {
            Id = gorsel.Id,
            SiparisId = gorsel.SiparisId,
            GorselUrl = gorsel.GorselUrl,
            YuklemeTarihi = gorsel.YuklemeTarihi,
            MusteriOnayladiMi = gorsel.MusteriOnayladiMi
        };
    }

    public async Task<Guid> GorselEkleVeyaGuncelleAsync(GorselOnayEkleDto dto)
    {
        var liste = await _gorselDeposu.GetirAsync(g => g.SiparisId == dto.SiparisId);
        var mevcut = liste.FirstOrDefault();

        if (mevcut == null)
        {
            var yeniGorsel = new SiparisGorselOnay
            {
                SiparisId = dto.SiparisId,
                GorselUrl = dto.GorselUrl,
                YuklemeTarihi = DateTime.UtcNow,
                MusteriOnayladiMi = false
            };

            await _gorselDeposu.EkleAsync(yeniGorsel);
            await _gorselDeposu.DegisiklikleriKaydetAsync();

            return yeniGorsel.Id;
        }
        else
        {
            mevcut.GorselUrl = dto.GorselUrl;
            mevcut.YuklemeTarihi = DateTime.UtcNow;
            _gorselDeposu.Guncelle(mevcut);
            await _gorselDeposu.DegisiklikleriKaydetAsync();

            return mevcut.Id;
        }
    }

    public async Task MusteriOnayiVerAsync(Guid siparisId)
    {
        var liste = await _gorselDeposu.GetirAsync(g => g.SiparisId == siparisId);
        var mevcut = liste.FirstOrDefault();

        if (mevcut != null)
        {
            mevcut.MusteriOnayladiMi = true;
            _gorselDeposu.Guncelle(mevcut);
            await _gorselDeposu.DegisiklikleriKaydetAsync();
        }
    }

    public async Task SilAsync(Guid siparisId)
    {
        var liste = await _gorselDeposu.GetirAsync(g => g.SiparisId == siparisId);
        var mevcut = liste.FirstOrDefault();

        if (mevcut != null)
        {
            _gorselDeposu.Sil(mevcut);
            await _gorselDeposu.DegisiklikleriKaydetAsync();
        }
    }
}