using ECommerce.Application.DTOs.Sayfa;

namespace ECommerce.Application.Interfaces;

public interface ISayfaServisi
{
    Task<IReadOnlyList<SayfaListeDto>> TumSayfalariGetirAsync();
    Task<SayfaGuncelleDto?> IdIleGetirAsync(Guid id);
    Task<SayfaGuncelleDto?> KodIleGetirAsync(string sayfaKodu);
    Task<Guid> EkleAsync(SayfaEkleDto dto);
    Task GuncelleAsync(SayfaGuncelleDto dto);
    Task SilAsync(Guid id);
}