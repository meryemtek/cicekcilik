using ECommerce.Application.DTOs.Kategori;

namespace ECommerce.Application.Interfaces;

public interface IKategoriServisi
{
    Task<IReadOnlyList<KategoriListeDto>> TumKategorileriGetirAsync();
    Task<KategoriGuncelleDto?> IdIleGetirAsync(Guid id);
    Task<Guid> EkleAsync(KategoriEkleDto dto);
    Task GuncelleAsync(KategoriGuncelleDto dto);
    Task SilAsync(Guid id);
}