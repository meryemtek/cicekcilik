using System.Threading.Tasks;
using ECommerce.Application.DTOs.Odeme;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers;

[Route("api/odeme")]
[ApiController]
public class OdemeController : ControllerBase
{
    private readonly IOdemeServisi _odemeServisi;

    public OdemeController(IOdemeServisi odemeServisi)
    {
        _odemeServisi = odemeServisi;
    }

    [HttpPost("baslat")]
    public async Task<IActionResult> OdemeBaslat([FromBody] OdemeIstekDto istek)
    {
        var sonuc = await _odemeServisi.OdemeBaslatAsync(istek);
        if (!sonuc.BasariliMi)
        {
            return BadRequest(new { Hata = sonuc.HataMesaji });
        }

        return Ok(sonuc);
    }
}