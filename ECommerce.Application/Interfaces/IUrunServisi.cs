using ECommerce.Application.DTOs.Urun;
using ECommerce.Application.DTOs.UrunYorum;

namespace ECommerce.Application.Interfaces;

public interface IUrunServisi
{
    Task<IReadOnlyList<UrunListeDto>> TumUrunleriGetirAsync();
    Task<UrunGuncelleDto?> IdIleGetirAsync(Guid id);
    Task<UrunDetayDto?> IdIleDetayGetirAsync(Guid id);
    Task<Guid> EkleAsync(UrunEkleDto dto);
    Task GuncelleAsync(UrunGuncelleDto dto);
    Task DurumDegistirAsync(Guid id, bool aktifMi);
    Task<List<UrunListeDto>> KategoriIdyeGoreGetirAsync(Guid kategoriId);
    Task SilAsync(Guid id);
    Task<UrunDetayDto?> SeoUrlIleDetayGetirAsync(string seoUrl);
    Task UrunYorumuEkleAsync(UrunYorumEkleDto dto);
}