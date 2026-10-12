using ECommerce.Application.DTOs.KartNotuSablonu;

namespace ECommerce.Application.Interfaces;

public interface IKartNotuSablonuServisi
{
    Task<IReadOnlyList<KartNotuSablonuListeDto>> TumSablonlariGetirAsync();
    Task<IReadOnlyList<KartNotuSablonuListeDto>> KategoriyeGoreGetirAsync(string kategori);
    Task<KartNotuSablonuGuncelleDto?> IdIleGetirAsync(Guid id);
    Task<Guid> EkleAsync(KartNotuSablonuEkleDto dto);
    Task GuncelleAsync(KartNotuSablonuGuncelleDto dto);
    Task SilAsync(Guid id);
}