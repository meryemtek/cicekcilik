using ECommerce.Application.DTOs.UrunVaryant;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers.Admin;

[Authorize]
[Route("api/admin/urun-varyantlari")]
[ApiController]
public class AdminUrunVaryantlariController : ControllerBase
{
    private readonly IUrunVaryantServisi _varyantServisi;

    public AdminUrunVaryantlariController(IUrunVaryantServisi varyantServisi)
    {
        _varyantServisi = varyantServisi;
    }

    [HttpGet("urun/{urunId:guid}")]
    public async Task<IActionResult> UrununVaryantlariniGetir(Guid urunId)
    {
        var varyantlar = await _varyantServisi.UrununVaryantlariniGetirAsync(urunId);
        return Ok(varyantlar);
    }

    [HttpPost]
    public async Task<IActionResult> Ekle([FromBody] UrunVaryantEkleDto dto)
    {
        var id = await _varyantServisi.EkleAsync(dto);
        return Ok(new { Id = id, Mesaj = "Ürün varyantı başarıyla eklendi." });
    }

    [HttpPut]
    public async Task<IActionResult> Guncelle([FromBody] UrunVaryantGuncelleDto dto)
    {
        await _varyantServisi.GuncelleAsync(dto);
        return Ok(new { Mesaj = "Ürün varyantı başarıyla güncellendi." });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Sil(Guid id)
    {
        await _varyantServisi.SilAsync(id);
        return Ok(new { Mesaj = "Ürün varyantı başarıyla silindi." });
    }
}