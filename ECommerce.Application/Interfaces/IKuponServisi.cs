using ECommerce.Application.DTOs.Kupon;

namespace ECommerce.Application.Interfaces;

public interface IKuponServisi
{
    Task<IReadOnlyList<KuponListeDto>> TumKuponlariGetirAsync();
    Task<KuponGuncelleDto?> IdIleGetirAsync(Guid id);
    Task<KuponListeDto?> KodIleGetirAsync(string kod);
    Task<Guid> EkleAsync(KuponEkleDto dto);
    Task GuncelleAsync(KuponGuncelleDto dto);
    Task SilAsync(Guid id);
}