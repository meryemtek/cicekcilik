using ECommerce.Application.DTOs.SemtTeslimatBolgesi;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services;

public class SemtTeslimatServisi : ISemtTeslimatServisi
{
    private readonly IDepo<SemtTeslimatBolgesi> _bolgeDeposu;

    public SemtTeslimatServisi(IDepo<SemtTeslimatBolgesi> bolgeDeposu)
    {
        _bolgeDeposu = bolgeDeposu;
    }

    public async Task<IReadOnlyList<SemtBolgeListeDto>> TumBolgeleriGetirAsync()
    {
        var bolgeler = await _bolgeDeposu.TumunuGetirAsync();

        return bolgeler.Select(b => new SemtBolgeListeDto
        {
            Id = b.Id,
            IlAdi = b.IlAdi,
            IlceAdi = b.IlceAdi,
            MahalleAdi = b.MahalleAdi,
            EkTeslimatUcreti = b.EkTeslimatUcreti,
            AktifMi = b.AktifMi
        }).OrderBy(b => b.IlAdi).ThenBy(b => b.IlceAdi).ThenBy(b => b.MahalleAdi).ToList();
    }

    // YENİ EKLENEN SAYFALAMA METODU
    public async Task<SemtBolgeSayfaliSonucDto> SayfaliGetirAsync(int start, int length, string aramaMetni)
    {
        var tumBolgeler = await _bolgeDeposu.TumunuGetirAsync();
        var query = tumBolgeler.AsQueryable();

        // Arama varsa filtrele
        if (!string.IsNullOrEmpty(aramaMetni))
        {
            var kucukArama = aramaMetni.ToLower();
            query = query.Where(x =>
                x.IlAdi.ToLower().Contains(kucukArama) ||
                x.IlceAdi.ToLower().Contains(kucukArama) ||
                x.MahalleAdi.ToLower().Contains(kucukArama));
        }

        int toplamKayit = tumBolgeler.Count;
        int filtrelenmisKayit = query.Count();

        var veriler = query
            .OrderBy(b => b.IlAdi)
            .ThenBy(b => b.IlceAdi)
            .ThenBy(b => b.MahalleAdi)
            .Skip(start)
            .Take(length)
            .Select(b => new SemtBolgeListeDto
            {
                Id = b.Id,
                IlAdi = b.IlAdi,
                IlceAdi = b.IlceAdi,
                MahalleAdi = b.MahalleAdi,
                EkTeslimatUcreti = b.EkTeslimatUcreti,
                AktifMi = b.AktifMi
            }).ToList();

        return new SemtBolgeSayfaliSonucDto
        {
            RecordsTotal = toplamKayit,
            RecordsFiltered = filtrelenmisKayit,
            Data = veriler
        };
    }

    public async Task<SemtBolgeGuncelleDto?> IdIleGetirAsync(Guid id)
    {
        var bolge = await _bolgeDeposu.IdIleGetirAsync(id);
        if (bolge == null) return null;

        return new SemtBolgeGuncelleDto
        {
            Id = bolge.Id,
            IlAdi = bolge.IlAdi,
            IlceAdi = bolge.IlceAdi,
            MahalleAdi = bolge.MahalleAdi,
            EkTeslimatUcreti = bolge.EkTeslimatUcreti,
            AktifMi = bolge.AktifMi
        };
    }

    public async Task<Guid> EkleAsync(SemtBolgeEkleDto dto)
    {
        var yeniBolge = new SemtTeslimatBolgesi
        {
            IlAdi = dto.IlAdi,
            IlceAdi = dto.IlceAdi,
            MahalleAdi = dto.MahalleAdi,
            EkTeslimatUcreti = dto.EkTeslimatUcreti,
            AktifMi = dto.AktifMi
        };

        await _bolgeDeposu.EkleAsync(yeniBolge);
        await _bolgeDeposu.DegisiklikleriKaydetAsync();

        return yeniBolge.Id;
    }

    public async Task GuncelleAsync(SemtBolgeGuncelleDto dto)
    {
        var bolge = await _bolgeDeposu.IdIleGetirAsync(dto.Id);
        if (bolge != null)
        {
            bolge.IlAdi = dto.IlAdi;
            bolge.IlceAdi = dto.IlceAdi;
            bolge.MahalleAdi = dto.MahalleAdi;
            bolge.EkTeslimatUcreti = dto.EkTeslimatUcreti;
            bolge.AktifMi = dto.AktifMi;
            bolge.GuncellenmeTarihi = DateTime.UtcNow;

            _bolgeDeposu.Guncelle(bolge);
            await _bolgeDeposu.DegisiklikleriKaydetAsync();
        }
    }

    public async Task SilAsync(Guid id)
    {
        var bolge = await _bolgeDeposu.IdIleGetirAsync(id);
        if (bolge != null)
        {
            _bolgeDeposu.Sil(bolge);
            await _bolgeDeposu.DegisiklikleriKaydetAsync();
        }
    }
}