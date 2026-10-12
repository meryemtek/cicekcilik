using ECommerce.Application.DTOs.KartNotuSablonu;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services;

public class KartNotuSablonuServisi : IKartNotuSablonuServisi
{
    private readonly IDepo<KartNotuSablonu> _sablonDeposu;

    public KartNotuSablonuServisi(IDepo<KartNotuSablonu> sablonDeposu)
    {
        _sablonDeposu = sablonDeposu;
    }

    public async Task<IReadOnlyList<KartNotuSablonuListeDto>> TumSablonlariGetirAsync()
    {
        var sablonlar = await _sablonDeposu.TumunuGetirAsync();

        return sablonlar.Select(s => new KartNotuSablonuListeDto
        {
            Id = s.Id,
            Kategori = s.Kategori,
            MesajIcerigi = s.MesajIcerigi,
            SiraNo = s.SiraNo,
            AktifMi = s.AktifMi
        }).OrderBy(s => s.Kategori).ThenBy(s => s.SiraNo).ToList();
    }

    public async Task<IReadOnlyList<KartNotuSablonuListeDto>> KategoriyeGoreGetirAsync(string kategori)
    {
        var sablonlar = await _sablonDeposu.GetirAsync(s => s.Kategori == kategori && s.AktifMi);

        return sablonlar.Select(s => new KartNotuSablonuListeDto
        {
            Id = s.Id,
            Kategori = s.Kategori,
            MesajIcerigi = s.MesajIcerigi,
            SiraNo = s.SiraNo,
            AktifMi = s.AktifMi
        }).OrderBy(s => s.SiraNo).ToList();
    }

    public async Task<KartNotuSablonuGuncelleDto?> IdIleGetirAsync(Guid id)
    {
        var sablon = await _sablonDeposu.IdIleGetirAsync(id);
        if (sablon == null) return null;

        return new KartNotuSablonuGuncelleDto
        {
            Id = sablon.Id,
            Kategori = sablon.Kategori,
            MesajIcerigi = sablon.MesajIcerigi,
            SiraNo = sablon.SiraNo,
            AktifMi = sablon.AktifMi
        };
    }

    public async Task<Guid> EkleAsync(KartNotuSablonuEkleDto dto)
    {
        var yeniSablon = new KartNotuSablonu
        {
            Kategori = dto.Kategori,
            MesajIcerigi = dto.MesajIcerigi,
            SiraNo = dto.SiraNo,
            AktifMi = dto.AktifMi
        };

        await _sablonDeposu.EkleAsync(yeniSablon);
        await _sablonDeposu.DegisiklikleriKaydetAsync();

        return yeniSablon.Id;
    }

    public async Task GuncelleAsync(KartNotuSablonuGuncelleDto dto)
    {
        var sablon = await _sablonDeposu.IdIleGetirAsync(dto.Id);
        if (sablon != null)
        {
            sablon.Kategori = dto.Kategori;
            sablon.MesajIcerigi = dto.MesajIcerigi;
            sablon.SiraNo = dto.SiraNo;
            sablon.AktifMi = dto.AktifMi;
            sablon.GuncellenmeTarihi = DateTime.UtcNow;

            _sablonDeposu.Guncelle(sablon);
            await _sablonDeposu.DegisiklikleriKaydetAsync();
        }
    }

    public async Task SilAsync(Guid id)
    {
        var sablon = await _sablonDeposu.IdIleGetirAsync(id);
        if (sablon != null)
        {
            _sablonDeposu.Sil(sablon);
            await _sablonDeposu.DegisiklikleriKaydetAsync();
        }
    }
}