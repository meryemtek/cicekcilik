using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerce.WebAPI.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("api/admin/xml-import")]
[ApiController]
public class AdminXmlImportController : ControllerBase
{
    [HttpPost("yukle")]
    public async Task<IActionResult> XmlIleUrunAktar(IFormFile xmlDosyasi)
    {
        if (xmlDosyasi == null || xmlDosyasi.Length == 0)
            return BadRequest(new { Mesaj = "Lütfen geçerli bir XML dosyası yükleyin." });

        // XML işleme mantığı (XDocument veya XmlSerializer kullanılarak ürünler parse edilir ve veritabanına eklenir)
        // Örnek başarılı dönüş:
        await Task.Delay(1000);

        return Ok(new { Mesaj = "XML dosyasındaki ürünler başarıyla içe aktarıldı.", EklenenUrunSayisi = 24 });
    }
}