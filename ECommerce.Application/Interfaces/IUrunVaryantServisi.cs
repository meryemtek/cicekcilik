using ECommerce.Application.DTOs.UrunVaryant;

namespace ECommerce.Application.Interfaces;

public interface IUrunVaryantServisi
{
    Task<IReadOnlyList<UrunVaryantListeDto>> UrununVaryantlariniGetirAsync(Guid urunId);
    Task<UrunVaryantGuncelleDto?> IdIleGetirAsync(Guid id);
    Task<Guid> EkleAsync(UrunVaryantEkleDto dto);
    Task GuncelleAsync(UrunVaryantGuncelleDto dto);
    Task SilAsync(Guid id);
}