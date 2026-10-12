using ECommerce.Application.DTOs.BankaHesabi;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services;

public class BankaHesabiServisi : IBankaHesabiServisi
{
    private readonly IDepo<BankaHesabi> _bankaDeposu;

    public BankaHesabiServisi(IDepo<BankaHesabi> bankaDeposu)
    {
        _bankaDeposu = bankaDeposu;
    }

    public async Task<IReadOnlyList<BankaHesabiListeDto>> TumBankaHesaplariniGetirAsync()
    {
        var hesaplar = await _bankaDeposu.TumunuGetirAsync();

        return hesaplar.Select(h => new BankaHesabiListeDto
        {
            Id = h.Id,
            BankaAdi = h.BankaAdi,
            Iban = h.Iban,
            AktifMi = h.AktifMi
        }).ToList();
    }

    public async Task<BankaHesabiGuncelleDto?> IdIleGetirAsync(Guid id)
    {
        var hesap = await _bankaDeposu.IdIleGetirAsync(id);
        if (hesap == null) return null;

        return new BankaHesabiGuncelleDto
        {
            Id = hesap.Id,
            BankaAdi = hesap.BankaAdi,
            SubeKodu = hesap.SubeKodu,
            HesapNo = hesap.HesapNo,
            Iban = hesap.Iban,
            AliciAdSoyad = hesap.AliciAdSoyad,
            AktifMi = hesap.AktifMi
        };
    }

    public async Task<Guid> EkleAsync(BankaHesabiEkleDto dto)
    {
        var yeniHesap = new BankaHesabi
        {
            BankaAdi = dto.BankaAdi,
            SubeKodu = dto.SubeKodu,
            HesapNo = dto.HesapNo,
            Iban = dto.Iban,
            AliciAdSoyad = dto.AliciAdSoyad,
            AktifMi = dto.AktifMi
        };

        await _bankaDeposu.EkleAsync(yeniHesap);
        await _bankaDeposu.DegisiklikleriKaydetAsync();

        return yeniHesap.Id;
    }

    public async Task GuncelleAsync(BankaHesabiGuncelleDto dto)
    {
        var hesap = await _bankaDeposu.IdIleGetirAsync(dto.Id);
        if (hesap != null)
        {
            hesap.BankaAdi = dto.BankaAdi;
            hesap.SubeKodu = dto.SubeKodu;
            hesap.HesapNo = dto.HesapNo;
            hesap.Iban = dto.Iban;
            hesap.AliciAdSoyad = dto.AliciAdSoyad;
            hesap.AktifMi = dto.AktifMi;
            hesap.GuncellenmeTarihi = DateTime.UtcNow;

            _bankaDeposu.Guncelle(hesap);
            await _bankaDeposu.DegisiklikleriKaydetAsync();
        }
    }

    public async Task SilAsync(Guid id)
    {
        var hesap = await _bankaDeposu.IdIleGetirAsync(id);
        if (hesap != null)
        {
            _bankaDeposu.Sil(hesap);
            await _bankaDeposu.DegisiklikleriKaydetAsync();
        }
    }

    public async Task<IReadOnlyList<BankaHesabiPublicDto>> AktifBankaHesaplariniGetirAsync()
    {
        var hesaplar = await _bankaDeposu.GetirAsync(h => h.AktifMi);

        return hesaplar.Select(h => new BankaHesabiPublicDto
        {
            Id = h.Id,
            BankaAdi = h.BankaAdi,
            SubeKodu = h.SubeKodu,
            HesapNo = h.HesapNo,
            Iban = h.Iban,
            AliciAdSoyad = h.AliciAdSoyad
        }).ToList();
    }
}