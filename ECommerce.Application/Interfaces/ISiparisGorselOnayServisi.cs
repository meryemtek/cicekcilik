using ECommerce.Application.DTOs.SiparisGorselOnay;

namespace ECommerce.Application.Interfaces;

public interface ISiparisGorselOnayServisi
{
    Task<SiparisGorselOnayDto?> SiparisGorselGetirAsync(Guid siparisId);
    Task<Guid> GorselEkleVeyaGuncelleAsync(GorselOnayEkleDto dto);
    Task MusteriOnayiVerAsync(Guid siparisId);
    Task SilAsync(Guid siparisId);
}