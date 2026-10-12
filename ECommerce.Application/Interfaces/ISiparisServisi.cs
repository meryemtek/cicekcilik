using ECommerce.Application.DTOs.Siparis;

namespace ECommerce.Application.Interfaces;

public interface ISiparisServisi
{
    Task<IReadOnlyList<SiparisListeDto>> TumSiparisleriGetirAsync();
    Task<IReadOnlyList<SiparisListeDto>> KullanicininSiparisleriniGetirAsync(Guid kullaniciId);
    Task<SiparisDetayDto?> SiparisDetayGetirAsync(Guid id);
    Task<Guid> SiparisOlusturAsync(SiparisOlusturDto dto);
    Task DurumGuncelleAsync(SiparisDurumGuncelleDto dto);
}