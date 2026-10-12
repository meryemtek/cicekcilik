using ECommerce.Application.DTOs.Blog;

namespace ECommerce.Application.Interfaces;

public interface IBlogServisi
{
    Task<IReadOnlyList<BlogListeDto>> TumYazilariGetirAsync();
    Task<BlogGuncelleDto?> IdIleGetirAsync(Guid id);
    Task<BlogGuncelleDto?> SeoUrlIleGetirAsync(string seoUrl); // Ön yüz detay sayfası için eklendi
    Task<Guid> EkleAsync(BlogEkleDto dto);
    Task GuncelleAsync(BlogGuncelleDto dto);
    Task SilAsync(Guid id);
}