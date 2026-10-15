using ECommerce.Application.DTOs.UrunYorum;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("api/admin/urun-yorumlari")]
[ApiController]
public class AdminUrunYorumlariController : ControllerBase
{
    private readonly IUrunYorumServisi _urunYorumServisi;

    public AdminUrunYorumlariController(IUrunYorumServisi urunYorumServisi)
    {
        _urunYorumServisi = urunYorumServisi;
    }

    [HttpGet]
    public async Task<IActionResult> TumunuGetir()
    {
        var yorumlar = await _urunYorumServisi.TumYorumlariGetirAsync();
        return Ok(yorumlar);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Getir(Guid id)
    {
        var yorum = await _urunYorumServisi.IdIleGetirAsync(id);
        if (yorum == null) return NotFound("Ürün yorumu bulunamadı.");
        return Ok(yorum);
    }

    [HttpPut]
    public async Task<IActionResult> Guncelle([FromBody] UrunYorumGuncelleDto dto)
    {
        await _urunYorumServisi.GuncelleAsync(dto);
        return Ok(new { Mesaj = "Ürün yorumu başarıyla güncellendi." });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Sil(Guid id)
    {
        await _urunYorumServisi.SilAsync(id);
        return Ok(new { Mesaj = "Ürün yorumu başarıyla silindi." });
    }
}