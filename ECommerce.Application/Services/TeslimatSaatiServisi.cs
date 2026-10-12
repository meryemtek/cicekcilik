using ECommerce.Application.DTOs.TeslimatSaati;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;

public class TeslimatSaatiServisi : ITeslimatSaatiServisi
{
    private readonly IDepo<TeslimatSaati> _depo;

    public TeslimatSaatiServisi(IDepo<TeslimatSaati> depo)
    {
        _depo = depo;
    }

    public async Task<List<TeslimatSaatiDto>> AktifSaatleriGetirAsync(TeslimatGrubu? grup = null)
    {
        var liste = await _depo.TumunuGetirAsync();

        var query = liste.Where(x => x.AktifMi);

        if (grup.HasValue)
        {
            query = query.Where(x => x.TeslimatGrubu == grup.Value);
        }

        return query.OrderBy(x => x.Sira)
                    .Select(x => new TeslimatSaatiDto
                    {
                        Id = x.Id,
                        BaslangicSaati = x.BaslangicSaati,
                        BitisSaati = x.BitisSaati,
                        Aciklama = x.Aciklama,
                        GunlukKota = x.GunlukKota,
                        AktifMi = x.AktifMi,
                        Sira = x.Sira,
                        TeslimatGrubu = x.TeslimatGrubu
                    }).ToList();
    }

    public async Task<List<TeslimatSaatiDto>> TumunuGetirAsync()
    {
        var liste = await _depo.TumunuGetirAsync();
        return liste.OrderBy(x => x.Sira).Select(x => new TeslimatSaatiDto
        {
            Id = x.Id,
            BaslangicSaati = x.BaslangicSaati,
            BitisSaati = x.BitisSaati,
            Aciklama = x.Aciklama,
            GunlukKota = x.GunlukKota,
            AktifMi = x.AktifMi,
            Sira = x.Sira,
            TeslimatGrubu = x.TeslimatGrubu
        }).ToList();
    }

    public async Task<TeslimatSaatiDto?> GetirByIdAsync(Guid id)
    {
        var item = await _depo.IdIleGetirAsync(id);
        if (item == null) return null;

        return new TeslimatSaatiDto
        {
            Id = item.Id,
            BaslangicSaati = item.BaslangicSaati,
            BitisSaati = item.BitisSaati,
            Aciklama = item.Aciklama,
            GunlukKota = item.GunlukKota,
            AktifMi = item.AktifMi,
            Sira = item.Sira,
            TeslimatGrubu = item.TeslimatGrubu
        };
    }

    public async Task EkleAsync(TeslimatSaatiDto dto)
    {
        var entity = new TeslimatSaati
        {
            BaslangicSaati = dto.BaslangicSaati,
            BitisSaati = dto.BitisSaati,
            Aciklama = string.IsNullOrEmpty(dto.Aciklama) ? $"{dto.BaslangicSaati} - {dto.BitisSaati}" : dto.Aciklama,
            GunlukKota = dto.GunlukKota,
            AktifMi = dto.AktifMi,
            Sira = dto.Sira,
            TeslimatGrubu = dto.TeslimatGrubu
        };
        await _depo.EkleAsync(entity);
        await _depo.DegisiklikleriKaydetAsync();
    }

    public async Task GuncelleAsync(TeslimatSaatiDto dto)
    {
        var entity = await _depo.IdIleGetirAsync(dto.Id);
        if (entity != null)
        {
            entity.BaslangicSaati = dto.BaslangicSaati;
            entity.BitisSaati = dto.BitisSaati;
            entity.Aciklama = dto.Aciklama;
            entity.GunlukKota = dto.GunlukKota;
            entity.AktifMi = dto.AktifMi;
            entity.Sira = dto.Sira;
            entity.TeslimatGrubu = dto.TeslimatGrubu;
            entity.GuncellenmeTarihi = DateTime.UtcNow;

            _depo.Guncelle(entity);
            await _depo.DegisiklikleriKaydetAsync();
        }
    }

    public async Task SilAsync(Guid id)
    {
        var entity = await _depo.IdIleGetirAsync(id);
        if (entity != null)
        {
            _depo.Sil(entity);
            await _depo.DegisiklikleriKaydetAsync();
        }
    }
}