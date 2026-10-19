using ECommerce.Application.DTOs.Siparis;

namespace ECommerce.Application.Interfaces;

public interface ISiparisServisi
{
    Task<IReadOnlyList<SiparisListeDto>> TumSiparisleriGetirAsync();
    Task<IReadOnlyList<SiparisListeDto>> KullanicininSiparisleriniGetirAsync(Guid kullaniciId);
    Task<SiparisDetayDto?> SiparisDetayGetirAsync(Guid id);
    Task<Guid> SiparisOlusturAsync(SiparisOlusturDto dto);
    Task<bool> PaytrSonucunuIsleAsync(string siparisNumarasi, string durum, long toplamTutar);
    Task OdemeBaslatmaIptaliniIsleAsync(Guid siparisId);
    Task<SiparisDurumGuncelleSonucDto> DurumGuncelleAsync(SiparisDurumGuncelleDto dto);
}
