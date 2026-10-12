using ECommerce.Application.DTOs.UrunYorum;

namespace ECommerce.Application.Interfaces;

public interface IUrunYorumServisi
{
    Task<IReadOnlyList<UrunYorumListeDto>> TumYorumlariGetirAsync();
    Task<IReadOnlyList<UrunYorumListeDto>> UrununYorumlariniGetirAsync(Guid urunId);
    Task<UrunYorumGuncelleDto?> IdIleGetirAsync(Guid id);
    Task<Guid> EkleAsync(UrunYorumEkleDto dto);
    Task GuncelleAsync(UrunYorumGuncelleDto dto);
    Task SilAsync(Guid id);
}