using ECommerce.Application.DTOs.SSS;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("api/admin/sss")]
[ApiController]
public class AdminSssController : ControllerBase
{
    private readonly ISssServisi _sssServisi;

    public AdminSssController(ISssServisi sssServisi)
    {
        _sssServisi = sssServisi;
    }

    [HttpGet]
    public async Task<IActionResult> TumunuGetir()
    {
        var sorular = await _sssServisi.TumSorulariGetirAsync();
        return Ok(sorular);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Getir(Guid id)
    {
        var soru = await _sssServisi.IdIleGetirAsync(id);
        if (soru == null) return NotFound("Soru bulunamadı.");
        return Ok(soru);
    }

    [HttpPost]
    public async Task<IActionResult> Ekle([FromBody] SssEkleDto dto)
    {
        var id = await _sssServisi.EkleAsync(dto);
        return Ok(new { Id = id, Mesaj = "Sık sorulan soru başarıyla eklendi." });
    }

    [HttpPut]
    public async Task<IActionResult> Guncelle([FromBody] SssGuncelleDto dto)
    {
        await _sssServisi.GuncelleAsync(dto);
        return Ok(new { Mesaj = "Sık sorulan soru başarıyla güncellendi." });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Sil(Guid id)
    {
        await _sssServisi.SilAsync(id);
        return Ok(new { Mesaj = "Sık sorulan soru başarıyla silindi." });
    }
}