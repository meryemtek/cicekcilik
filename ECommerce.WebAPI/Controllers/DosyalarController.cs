using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers;

[ApiController]
[Route("api/dosyalar")]
public class DosyalarController : ControllerBase
{
    private readonly IDosyaServisi _dosyaServisi;

    public DosyalarController(IDosyaServisi dosyaServisi)
    {
        _dosyaServisi = dosyaServisi;
    }

    [AllowAnonymous]
    [HttpGet("gorseller/{**anahtar}")]
    public async Task<IActionResult> Gorsel(string anahtar)
    {
        var gorsel = await _dosyaServisi.GorselOkuAsync(anahtar);
        if (gorsel is null)
        {
            return NotFound();
        }

        Response.Headers.CacheControl = "public, max-age=86400";
        return File(gorsel.Value.Stream, gorsel.Value.ContentType);
    }
}
