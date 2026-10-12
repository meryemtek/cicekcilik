using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers;

[Route("api/gorsel-onay")]
[ApiController]
public class GorselOnayController : ControllerBase
{
    private readonly ISiparisGorselOnayServisi _gorselServisi;

    public GorselOnayController(ISiparisGorselOnayServisi gorselServisi)
    {
        _gorselServisi = gorselServisi;
    }

    // Müşterinin SMS'teki linke tıklayıp çiçeğin fotoğrafını gördüğü uç nokta (Auth istemez)
    [HttpGet("{siparisId:guid}")]
    public async Task<IActionResult> GorseliGoruntule(Guid siparisId)
    {
        var gorsel = await _gorselServisi.SiparisGorselGetirAsync(siparisId);
        if (gorsel == null) return NotFound("Bu siparişe ait henüz kurye görseli yüklenmemiş.");
        return Ok(gorsel);
    }

    // Müşterinin "Onaylıyorum" butonuna bastığı uç nokta
    [HttpPost("{siparisId:guid}/onay-ver")]
    public async Task<IActionResult> OnayVer(Guid siparisId)
    {
        await _gorselServisi.MusteriOnayiVerAsync(siparisId);
        return Ok(new { Mesaj = "Çiçek görseli onaylandı. Teşekkür ederiz." });
    }
}