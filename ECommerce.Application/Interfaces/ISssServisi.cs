using ECommerce.Application.DTOs.SSS;

namespace ECommerce.Application.Interfaces;

public interface ISssServisi
{
    Task<IReadOnlyList<SssListeDto>> TumSorulariGetirAsync();
    Task<SssGuncelleDto?> IdIleGetirAsync(Guid id);
    Task<Guid> EkleAsync(SssEkleDto dto);
    Task GuncelleAsync(SssGuncelleDto dto);
    Task SilAsync(Guid id);
}