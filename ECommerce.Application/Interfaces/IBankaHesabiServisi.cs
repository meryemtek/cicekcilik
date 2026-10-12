using ECommerce.Application.DTOs.BankaHesabi;

namespace ECommerce.Application.Interfaces;

public interface IBankaHesabiServisi
{
    Task<IReadOnlyList<BankaHesabiListeDto>> TumBankaHesaplariniGetirAsync();
    Task<IReadOnlyList<BankaHesabiPublicDto>> AktifBankaHesaplariniGetirAsync(); // Yeni Eklenen Metot
    Task<BankaHesabiGuncelleDto?> IdIleGetirAsync(Guid id);
    Task<Guid> EkleAsync(BankaHesabiEkleDto dto);
    Task GuncelleAsync(BankaHesabiGuncelleDto dto);
    Task SilAsync(Guid id);
}