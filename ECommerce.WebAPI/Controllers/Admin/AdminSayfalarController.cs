using ECommerce.Application.DTOs.Sayfa;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers.Admin;

[Authorize]
[Route("api/admin/sayfalar")]
[ApiController]
public class AdminSayfalarController : ControllerBase
{
    private readonly ISayfaServisi _sayfaServisi;

    public AdminSayfalarController(ISayfaServisi sayfaServisi)
    {
        _sayfaServisi = sayfaServisi;
    }

    [HttpGet]
    public async Task<IActionResult> TumunuGetir()
    {
        var sayfalar = await _sayfaServisi.TumSayfalariGetirAsync();
        return Ok(sayfalar);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Getir(Guid id)
    {
        var sayfa = await _sayfaServisi.IdIleGetirAsync(id);
        if (sayfa == null) return NotFound("Sayfa bulunamadı.");
        return Ok(sayfa);
    }

    [HttpPost]
    public async Task<IActionResult> Ekle([FromBody] SayfaEkleDto dto)
    {
        var id = await _sayfaServisi.EkleAsync(dto);
        return Ok(new { Id = id, Mesaj = "Sayfa başarıyla eklendi." });
    }

    [HttpPut]
    public async Task<IActionResult> Guncelle([FromBody] SayfaGuncelleDto dto)
    {
        await _sayfaServisi.GuncelleAsync(dto);
        return Ok(new { Mesaj = "Sayfa başarıyla güncellendi." });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Sil(Guid id)
    {
        await _sayfaServisi.SilAsync(id);
        return Ok(new { Mesaj = "Sayfa başarıyla silindi." });
    }
}