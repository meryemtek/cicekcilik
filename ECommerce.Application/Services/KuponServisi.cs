using ECommerce.Application.DTOs.Kupon;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services;

public class KuponServisi : IKuponServisi
{
    private readonly IDepo<Kupon> _kuponDeposu;

    public KuponServisi(IDepo<Kupon> kuponDeposu)
    {
        _kuponDeposu = kuponDeposu;
    }

    public async Task<IReadOnlyList<KuponListeDto>> TumKuponlariGetirAsync()
    {
        var kuponlar = await _kuponDeposu.TumunuGetirAsync();

        return kuponlar.Select(k => new KuponListeDto
        {
            Id = k.Id,
            Kod = k.Kod,
            IndirimMiktari = k.IndirimMiktari,
            YuzdelikMi = k.YuzdelikMi,
            BitisTarihi = k.BitisTarihi,
            AktifMi = k.AktifMi
        }).ToList();
    }

    public async Task<KuponGuncelleDto?> IdIleGetirAsync(Guid id)
    {
        var kupon = await _kuponDeposu.IdIleGetirAsync(id);
        if (kupon == null) return null;

        return new KuponGuncelleDto
        {
            Id = kupon.Id,
            Kod = kupon.Kod,
            IndirimMiktari = kupon.IndirimMiktari,
            YuzdelikMi = kupon.YuzdelikMi,
            BitisTarihi = kupon.BitisTarihi,
            AktifMi = kupon.AktifMi
        };
    }
    public async Task<KuponListeDto?> KodIleGetirAsync(string kod)
    {
        var kuponlar = await _kuponDeposu.TumunuGetirAsync();
        var kupon = kuponlar.FirstOrDefault(k => k.Kod.Equals(kod.Trim(), StringComparison.OrdinalIgnoreCase));
        if (kupon == null) return null;

        return new KuponListeDto
        {
            Id = kupon.Id,
            Kod = kupon.Kod,
            IndirimMiktari = kupon.IndirimMiktari,
            YuzdelikMi = kupon.YuzdelikMi,
            BitisTarihi = kupon.BitisTarihi,
            AktifMi = kupon.AktifMi
        };
    }
    public async Task<Guid> EkleAsync(KuponEkleDto dto)
    {
        var yeniKupon = new Kupon
        {
            Kod = dto.Kod,
            IndirimMiktari = dto.IndirimMiktari,
            YuzdelikMi = dto.YuzdelikMi,
            BitisTarihi = dto.BitisTarihi,
            AktifMi = dto.AktifMi
        };

        await _kuponDeposu.EkleAsync(yeniKupon);
        await _kuponDeposu.DegisiklikleriKaydetAsync();

        return yeniKupon.Id;
    }

    public async Task GuncelleAsync(KuponGuncelleDto dto)
    {
        var kupon = await _kuponDeposu.IdIleGetirAsync(dto.Id);
        if (kupon != null)
        {
            kupon.Kod = dto.Kod;
            kupon.IndirimMiktari = dto.IndirimMiktari;
            kupon.YuzdelikMi = dto.YuzdelikMi;
            kupon.BitisTarihi = dto.BitisTarihi;
            kupon.AktifMi = dto.AktifMi;
            kupon.GuncellenmeTarihi = DateTime.UtcNow;

            _kuponDeposu.Guncelle(kupon);
            await _kuponDeposu.DegisiklikleriKaydetAsync();
        }
    }

    public async Task SilAsync(Guid id)
    {
        var kupon = await _kuponDeposu.IdIleGetirAsync(id);
        if (kupon != null)
        {
            _kuponDeposu.Sil(kupon);
            await _kuponDeposu.DegisiklikleriKaydetAsync();
        }
    }
}