using ECommerce.Application.DTOs.Adres;

namespace ECommerce.Application.Interfaces;

public interface IKayitliAdresServisi
{
    Task<IReadOnlyList<KayitliAdresDto>> ListeleAsync(Guid kullaniciId);
    Task<KayitliAdresDto?> GetirAsync(Guid kullaniciId, Guid id);
    Task<KayitliAdresDto> EkleAsync(Guid kullaniciId, KayitliAdresKaydetDto dto);
    Task<bool> GuncelleAsync(Guid kullaniciId, Guid id, KayitliAdresKaydetDto dto);
    Task<bool> SilAsync(Guid kullaniciId, Guid id);
    Task<bool> VarsayilanYapAsync(Guid kullaniciId, Guid id);
}
