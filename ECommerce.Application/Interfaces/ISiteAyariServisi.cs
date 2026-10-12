using System.Threading.Tasks;
using ECommerce.Application.DTOs.SiteAyari;

namespace ECommerce.Application.Interfaces
{
    public interface ISiteAyariServisi
    {
        Task<SiteAyariDto> AyarlariGetirAsync();
        Task AyarlariGuncelleAsync(SiteAyariDto dto);
    }
}