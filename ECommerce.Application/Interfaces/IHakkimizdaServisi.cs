using ECommerce.Application.DTOs.Hakkimizda;

namespace ECommerce.Application.Interfaces
{
    public interface IHakkimizdaServisi
    {
        Task<HakkimizdaAyariDto> GetirAsync();
        Task GuncelleAsync(HakkimizdaAyariDto dto);
    }
}