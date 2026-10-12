using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces;

public interface IDosyaServisi
{
    Task<string> GorselYukleAsync(IFormFile dosya, string altKlasor);
    void GorselSil(string gorselUrl);
}