using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers.Admin;

[Authorize]
[Route("api/admin/iletisim-mesajlari")]
[ApiController]
public class AdminIletisimMesajiController : ControllerBase
{
    private readonly IIletisimMesajiServisi _iletisimMesajiServisi;

    public AdminIletisimMesajiController(IIletisimMesajiServisi iletisimMesajiServisi)
    {
        _iletisimMesajiServisi = iletisimMesajiServisi;
    }

    [HttpGet]
    public async Task<IActionResult> TumunuGetir()
    {
        var mesajlar = await _iletisimMesajiServisi.TumMesajlariGetirAsync();
        return Ok(mesajlar);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> DetayGetir(Guid id)
    {
        var mesaj = await _iletisimMesajiServisi.MesajOkuAsync(id);
        if (mesaj == null) return NotFound("İletişim mesajı bulunamadı.");
        return Ok(mesaj);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Sil(Guid id)
    {
        await _iletisimMesajiServisi.SilAsync(id);
        return Ok(new { Mesaj = "İletişim mesajı başarıyla silindi." });
    }

    [HttpGet("okunmamis-sayi")]
    public async Task<IActionResult> OkunmamisSayisiGetir()
    {
        var sayi = await _iletisimMesajiServisi.OkunmamisMesajSayisiGetirAsync();
        return Ok(sayi);
    }
}