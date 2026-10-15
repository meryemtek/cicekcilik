using ECommerce.Application.DTOs.Adres;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services;

public class KayitliAdresServisi : IKayitliAdresServisi
{
    private readonly IDepo<KayitliAdres> _depo;

    public KayitliAdresServisi(IDepo<KayitliAdres> depo) => _depo = depo;

    public async Task<IReadOnlyList<KayitliAdresDto>> ListeleAsync(Guid kullaniciId)
    {
        var adresler = await _depo.GetirAsync(a => a.KullaniciId == kullaniciId);
        return adresler.OrderByDescending(a => a.VarsayilanMi).ThenBy(a => a.OlusturulmaTarihi)
            .Select(Dto).ToList();
    }

    public async Task<KayitliAdresDto?> GetirAsync(Guid kullaniciId, Guid id)
    {
        var adres = (await _depo.GetirAsync(a => a.Id == id && a.KullaniciId == kullaniciId)).FirstOrDefault();
        return adres == null ? null : Dto(adres);
    }

    public async Task<KayitliAdresDto> EkleAsync(Guid kullaniciId, KayitliAdresKaydetDto dto)
    {
        var adresler = await _depo.GetirAsync(a => a.KullaniciId == kullaniciId);
        var adres = new KayitliAdres { KullaniciId = kullaniciId };
        Doldur(adres, dto);
        if (dto.VarsayilanMi || adresler.Count == 0)
        {
            foreach (var diger in adresler) diger.VarsayilanMi = false;
            adres.VarsayilanMi = true;
        }
        await _depo.EkleAsync(adres);
        await _depo.DegisiklikleriKaydetAsync();
        return Dto(adres);
    }

    public async Task<bool> GuncelleAsync(Guid kullaniciId, Guid id, KayitliAdresKaydetDto dto)
    {
        var adresler = await _depo.GetirAsync(a => a.KullaniciId == kullaniciId);
        var adres = adresler.FirstOrDefault(a => a.Id == id);
        if (adres == null) return false;
        Doldur(adres, dto);
        if (dto.VarsayilanMi)
        {
            foreach (var diger in adresler) diger.VarsayilanMi = diger.Id == id;
        }
        adres.GuncellenmeTarihi = DateTime.UtcNow;
        await _depo.DegisiklikleriKaydetAsync();
        return true;
    }

    public async Task<bool> SilAsync(Guid kullaniciId, Guid id)
    {
        var adresler = await _depo.GetirAsync(a => a.KullaniciId == kullaniciId);
        var adres = adresler.FirstOrDefault(a => a.Id == id);
        if (adres == null) return false;
        _depo.Sil(adres);
        if (adres.VarsayilanMi)
        {
            var sonraki = adresler.FirstOrDefault(a => a.Id != id);
            if (sonraki != null) sonraki.VarsayilanMi = true;
        }
        await _depo.DegisiklikleriKaydetAsync();
        return true;
    }

    public async Task<bool> VarsayilanYapAsync(Guid kullaniciId, Guid id)
    {
        var adresler = await _depo.GetirAsync(a => a.KullaniciId == kullaniciId);
        if (!adresler.Any(a => a.Id == id)) return false;
        foreach (var adres in adresler) adres.VarsayilanMi = adres.Id == id;
        await _depo.DegisiklikleriKaydetAsync();
        return true;
    }

    private static void Doldur(KayitliAdres adres, KayitliAdresKaydetDto dto)
    {
        adres.Baslik = dto.Baslik.Trim();
        adres.AliciAdSoyad = dto.AliciAdSoyad.Trim();
        adres.Telefon = dto.Telefon.Trim();
        adres.Il = dto.Il.Trim();
        adres.Ilce = dto.Ilce.Trim();
        adres.AcikAdres = dto.AcikAdres.Trim();
    }

    private static KayitliAdresDto Dto(KayitliAdres adres) => new()
    {
        Id = adres.Id, Baslik = adres.Baslik, AliciAdSoyad = adres.AliciAdSoyad,
        Telefon = adres.Telefon, Il = adres.Il, Ilce = adres.Ilce,
        AcikAdres = adres.AcikAdres, VarsayilanMi = adres.VarsayilanMi
    };
}
