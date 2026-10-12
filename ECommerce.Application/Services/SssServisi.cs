using ECommerce.Application.DTOs.SSS;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services;

public class SssServisi : ISssServisi
{
    private readonly IDepo<SikSorulanSoru> _sssDeposu;

    public SssServisi(IDepo<SikSorulanSoru> sssDeposu)
    {
        _sssDeposu = sssDeposu;
    }

    public async Task<IReadOnlyList<SssListeDto>> TumSorulariGetirAsync()
    {
        var sorular = await _sssDeposu.TumunuGetirAsync();

        return sorular.Select(s => new SssListeDto
        {
            Id = s.Id,
            Soru = s.Soru,
            SiraNo = s.SiraNo,
            AktifMi = s.AktifMi
        }).OrderBy(s => s.SiraNo).ToList();
    }

    public async Task<SssGuncelleDto?> IdIleGetirAsync(Guid id)
    {
        var soru = await _sssDeposu.IdIleGetirAsync(id);
        if (soru == null) return null;

        return new SssGuncelleDto
        {
            Id = soru.Id,
            Soru = soru.Soru,
            Cevap = soru.Cevap,
            SiraNo = soru.SiraNo,
            AktifMi = soru.AktifMi
        };
    }

    public async Task<Guid> EkleAsync(SssEkleDto dto)
    {
        var yeniSoru = new SikSorulanSoru
        {
            Soru = dto.Soru,
            Cevap = dto.Cevap,
            SiraNo = dto.SiraNo,
            AktifMi = dto.AktifMi
        };

        await _sssDeposu.EkleAsync(yeniSoru);
        await _sssDeposu.DegisiklikleriKaydetAsync();

        return yeniSoru.Id;
    }

    public async Task GuncelleAsync(SssGuncelleDto dto)
    {
        var soru = await _sssDeposu.IdIleGetirAsync(dto.Id);
        if (soru != null)
        {
            soru.Soru = dto.Soru;
            soru.Cevap = dto.Cevap;
            soru.SiraNo = dto.SiraNo;
            soru.AktifMi = dto.AktifMi;
            soru.GuncellenmeTarihi = DateTime.UtcNow;

            _sssDeposu.Guncelle(soru);
            await _sssDeposu.DegisiklikleriKaydetAsync();
        }
    }

    public async Task SilAsync(Guid id)
    {
        var soru = await _sssDeposu.IdIleGetirAsync(id);
        if (soru != null)
        {
            _sssDeposu.Sil(soru);
            await _sssDeposu.DegisiklikleriKaydetAsync();
        }
    }
}