using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces;

public interface IDosyaServisi
{
    Task<string> GorselYukleAsync(IFormFile dosya, string altKlasor);
    Task<string> DosyaYukleAsync(IFormFile dosya, string altKlasor);
    Task<(Stream Stream, string ContentType, string FileName)?> DosyaOkuAsync(string dosyaYolu);
    Task<(Stream Stream, string ContentType)?> GorselOkuAsync(string anahtar);
    void GorselSil(string gorselUrl);
}
