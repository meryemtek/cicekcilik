using ECommerce.Application.DTOs.SiparisGorselOnay;

namespace ECommerce.Application.Interfaces;

public interface ISiparisGorselOnayServisi
{
    Task<SiparisGorselOnayDto?> SiparisGorselGetirAsync(Guid siparisId);
    Task MusteriOnayiVerAsync(Guid siparisId);
}
