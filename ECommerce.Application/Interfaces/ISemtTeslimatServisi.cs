using ECommerce.Application.DTOs.SemtTeslimatBolgesi;

namespace ECommerce.Application.Interfaces;

public interface ISemtTeslimatServisi
{
    Task<IReadOnlyList<SemtBolgeListeDto>> TumBolgeleriGetirAsync();
    Task<SemtBolgeSayfaliSonucDto> SayfaliGetirAsync(int start, int length, string aramaMetni);
    Task<SemtBolgeGuncelleDto?> IdIleGetirAsync(Guid id);
    Task<Guid> EkleAsync(SemtBolgeEkleDto dto);
    Task GuncelleAsync(SemtBolgeGuncelleDto dto);
    Task SilAsync(Guid id);
}