using ECommerce.Application.DTOs.SiparisGorselOnay;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("api/admin/siparis-gorsel")]
[ApiController]
public class AdminSiparisGorselController : ControllerBase
{
    private readonly ISiparisGorselOnayServisi _gorselServisi;

    public AdminSiparisGorselController(ISiparisGorselOnayServisi gorselServisi)
    {
        _gorselServisi = gorselServisi;
    }

    [HttpGet("{siparisId:guid}")]
    public async Task<IActionResult> GorselGetir(Guid siparisId)
    {
        var gorsel = await _gorselServisi.SiparisGorselGetirAsync(siparisId);
        if (gorsel == null) return NotFound("Bu siparişe ait görsel onay kaydı bulunamadı.");
        return Ok(gorsel);
    }

    [HttpPost]
    public async Task<IActionResult> GorselYukleVeyaGuncelle([FromBody] GorselOnayEkleDto dto)
    {
        var id = await _gorselServisi.GorselEkleVeyaGuncelleAsync(dto);
        return Ok(new { Id = id, Mesaj = "Kurye görseli başarıyla yüklendi/güncellendi." });
    }

    [HttpPost("{siparisId:guid}/onayla")]
    public async Task<IActionResult> MusteriOnayiVer(Guid siparisId)
    {
        await _gorselServisi.MusteriOnayiVerAsync(siparisId);
        return Ok(new { Mesaj = "Müşteri görsel onay durumu güncellendi." });
    }

    [HttpDelete("{siparisId:guid}")]
    public async Task<IActionResult> Sil(Guid siparisId)
    {
        await _gorselServisi.SilAsync(siparisId);
        return Ok(new { Mesaj = "Görsel onay kaydı silindi." });
    }
}