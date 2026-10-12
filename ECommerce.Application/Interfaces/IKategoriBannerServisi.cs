using ECommerce.Application.DTOs.KategoriBanner;

namespace ECommerce.Application.Interfaces
{
    public interface IKategoriBannerServisi
    {
        Task<List<KategoriBannerDto>> TumunuGetirAsync();
        Task<KategoriBannerEkleGuncelleDto> GetirByIdAsync(Guid id);
        Task EkleAsync(KategoriBannerEkleGuncelleDto dto);
        Task GuncelleAsync(KategoriBannerEkleGuncelleDto dto);
        Task SilAsync(Guid id);
    }
}