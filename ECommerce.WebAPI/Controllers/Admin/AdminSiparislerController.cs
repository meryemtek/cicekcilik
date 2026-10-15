using ECommerce.Application.DTOs.Siparis;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("api/admin/siparisler")]
[ApiController]
public class AdminSiparislerController : ControllerBase
{
    private readonly ISiparisServisi _siparisServisi;

    public AdminSiparislerController(ISiparisServisi siparisServisi)
    {
        _siparisServisi = siparisServisi;
    }

    [HttpGet]
    public async Task<IActionResult> TumunuGetir()
    {
        var siparisler = await _siparisServisi.TumSiparisleriGetirAsync();
        return Ok(siparisler);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> DetayGetir(Guid id)
    {
        var detay = await _siparisServisi.SiparisDetayGetirAsync(id);
        if (detay == null) return NotFound("Sipariş bulunamadı.");
        return Ok(detay);
    }

    [HttpPut("durum-guncelle")]
    public async Task<IActionResult> DurumGuncelle([FromBody] SiparisDurumGuncelleDto dto)
    {
        await _siparisServisi.DurumGuncelleAsync(dto);
        return Ok(new { Mesaj = "Sipariş durumu başarıyla güncellendi." });
    }
}
