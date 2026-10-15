using ECommerce.Application.DTOs.KartNotuSablonu;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("api/admin/kart-notlari")]
[ApiController]
public class AdminKartNotlariController : ControllerBase
{
    private readonly IKartNotuSablonuServisi _kartNotuServisi;

    public AdminKartNotlariController(IKartNotuSablonuServisi kartNotuServisi)
    {
        _kartNotuServisi = kartNotuServisi;
    }

    [HttpGet]
    public async Task<IActionResult> TumunuGetir()
    {
        var sablonlar = await _kartNotuServisi.TumSablonlariGetirAsync();
        return Ok(sablonlar);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Getir(Guid id)
    {
        var sablon = await _kartNotuServisi.IdIleGetirAsync(id);
        if (sablon == null) return NotFound("Kart notu şablonu bulunamadı.");
        return Ok(sablon);
    }

    [HttpPost]
    public async Task<IActionResult> Ekle([FromBody] KartNotuSablonuEkleDto dto)
    {
        var id = await _kartNotuServisi.EkleAsync(dto);
        return Ok(new { Id = id, Mesaj = "Kart notu şablonu başarıyla eklendi." });
    }

    [HttpPut]
    public async Task<IActionResult> Guncelle([FromBody] KartNotuSablonuGuncelleDto dto)
    {
        await _kartNotuServisi.GuncelleAsync(dto);
        return Ok(new { Mesaj = "Kart notu şablonu başarıyla güncellendi." });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Sil(Guid id)
    {
        await _kartNotuServisi.SilAsync(id);
        return Ok(new { Mesaj = "Kart notu şablonu başarıyla silindi." });
    }
}