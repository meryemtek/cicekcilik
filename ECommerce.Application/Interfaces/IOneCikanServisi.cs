using ECommerce.Application.DTOs.OneCikan;
using ECommerce.Application.DTOs.Urun;

namespace ECommerce.Application.Interfaces
{
    public interface IOneCikanServisi
    {
        Task<OneCikanDto> AyarlariGetirAsync();
        Task AyarlariGuncelleAsync(OneCikanDto dto);
        Task<List<UrunListeDto>> OneCikanUrunleriGetirAsync();
    }
}