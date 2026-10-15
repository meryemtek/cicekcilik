using System;
using System.Threading.Tasks;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("api/admin/dosyalar")]
[ApiController]
public class AdminDosyalarController : ControllerBase
{
    private readonly IDosyaServisi _dosyaServisi;

    public AdminDosyalarController(IDosyaServisi dosyaServisi)
    {
        _dosyaServisi = dosyaServisi;
    }

    [HttpPost("gorsel-yukle")]
    public async Task<IActionResult> GorselYukle([FromForm] IFormFile dosya, [FromForm] string altKlasor = "genel")
    {
        try
        {
            // altKlasor parametresi ile "urunler", "slider", "kurye-onay" gibi klasör ayrımı yapabiliriz
            var gorselUrl = await _dosyaServisi.GorselYukleAsync(dosya, altKlasor);

            return Ok(new
            {
                Url = gorselUrl,
                Mesaj = "Görsel başarıyla sunucuya yüklendi."
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Hata = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Hata = "Görsel yüklenirken sunucu tarafında bir hata oluştu.", Detay = ex.Message });
        }
    }

    [HttpDelete("gorsel-sil")]
    public IActionResult GorselSil([FromBody] string gorselUrl)
    {
        try
        {
            _dosyaServisi.GorselSil(gorselUrl);
            return Ok(new { Mesaj = "Görsel başarıyla silindi." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Hata = "Görsel silinirken bir hata oluştu.", Detay = ex.Message });
        }
    }
}