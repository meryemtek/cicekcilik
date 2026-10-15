using ECommerce.Application.DTOs.Kategori;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class KategorilerController : ControllerBase
{
    private readonly IKategoriServisi _kategoriServisi;

    public KategorilerController(IKategoriServisi kategoriServisi)
    {
        _kategoriServisi = kategoriServisi;
    }

    [HttpGet]
    public async Task<IActionResult> TumunuGetir()
    {
        var kategoriler = await _kategoriServisi.TumKategorileriGetirAsync();
        return Ok(kategoriler);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Getir(Guid id)
    {
        var kategori = await _kategoriServisi.IdIleGetirAsync(id);
        if (kategori == null) return NotFound("Kategori bulunamadı.");
        return Ok(kategori);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Ekle([FromBody] KategoriEkleDto dto)
    {
        var id = await _kategoriServisi.EkleAsync(dto);
        return Ok(new { Id = id, Mesaj = "Kategori başarıyla eklendi." });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut]
    public async Task<IActionResult> Guncelle([FromBody] KategoriGuncelleDto dto)
    {
        await _kategoriServisi.GuncelleAsync(dto);
        return Ok(new { Mesaj = "Kategori başarıyla güncellendi." });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Sil(Guid id)
    {
        await _kategoriServisi.SilAsync(id);
        return Ok(new { Mesaj = "Kategori başarıyla silindi." });
    }
}