using ECommerce.Application.DTOs.UrunYorum;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services;

public class UrunYorumServisi : IUrunYorumServisi
{
    private readonly IDepo<UrunYorum> _yorumDeposu;

    public UrunYorumServisi(IDepo<UrunYorum> yorumDeposu)
    {
        _yorumDeposu = yorumDeposu;
    }

    public async Task<IReadOnlyList<UrunYorumListeDto>> TumYorumlariGetirAsync()
    {
        var yorumlar = await _yorumDeposu.TumunuGetirAsync();

        return yorumlar.Select(y => new UrunYorumListeDto
        {
            Id = y.Id,
            UrunId = y.UrunId,
            YorumcuAd = y.YorumcuAd,
            Puan = y.Puan,
            OnaylandiMi = y.OnaylandiMi,
            OlusturulmaTarihi = y.OlusturulmaTarihi
        }).OrderByDescending(y => y.OlusturulmaTarihi).ToList();
    }

    public async Task<IReadOnlyList<UrunYorumListeDto>> UrununYorumlariniGetirAsync(Guid urunId)
    {
        var yorumlar = await _yorumDeposu.GetirAsync(y => y.UrunId == urunId && y.OnaylandiMi);

        return yorumlar.Select(y => new UrunYorumListeDto
        {
            Id = y.Id,
            UrunId = y.UrunId,
            YorumcuAd = y.YorumcuAd,
            Puan = y.Puan,
            OnaylandiMi = y.OnaylandiMi,
            OlusturulmaTarihi = y.OlusturulmaTarihi
        }).OrderByDescending(y => y.OlusturulmaTarihi).ToList();
    }

    public async Task<UrunYorumGuncelleDto?> IdIleGetirAsync(Guid id)
    {
        var yorum = await _yorumDeposu.IdIleGetirAsync(id);
        if (yorum == null) return null;

        return new UrunYorumGuncelleDto
        {
            Id = yorum.Id,
            UrunId = yorum.UrunId,
            YorumcuAd = yorum.YorumcuAd,
            YorumMetni = yorum.YorumMetni,
            Puan = yorum.Puan,
            OnaylandiMi = yorum.OnaylandiMi
        };
    }

    public async Task<Guid> EkleAsync(UrunYorumEkleDto dto)
    {
        var yeniYorum = new UrunYorum
        {
            UrunId = dto.UrunId,
            YorumcuAd = dto.YorumcuAd,
            YorumMetni = dto.YorumMetni,
            Puan = dto.Puan,
            OnaylandiMi = false // Varsayılan olarak admin onayına düşmesi için false yapıyoruz
        };

        await _yorumDeposu.EkleAsync(yeniYorum);
        await _yorumDeposu.DegisiklikleriKaydetAsync();

        return yeniYorum.Id;
    }

    public async Task GuncelleAsync(UrunYorumGuncelleDto dto)
    {
        var yorum = await _yorumDeposu.IdIleGetirAsync(dto.Id);
        if (yorum != null)
        {
            yorum.YorumcuAd = dto.YorumcuAd;
            yorum.YorumMetni = dto.YorumMetni;
            yorum.Puan = dto.Puan;
            yorum.OnaylandiMi = dto.OnaylandiMi;
            yorum.GuncellenmeTarihi = DateTime.UtcNow;

            _yorumDeposu.Guncelle(yorum);
            await _yorumDeposu.DegisiklikleriKaydetAsync();
        }
    }

    public async Task SilAsync(Guid id)
    {
        var yorum = await _yorumDeposu.IdIleGetirAsync(id);
        if (yorum != null)
        {
            _yorumDeposu.Sil(yorum);
            await _yorumDeposu.DegisiklikleriKaydetAsync();
        }
    }
}