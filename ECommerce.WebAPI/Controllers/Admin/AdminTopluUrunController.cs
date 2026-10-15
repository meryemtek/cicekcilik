using ECommerce.Application.DTOs.Urun;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("api/admin/toplu-urun")]
[ApiController]
public class AdminTopluUrunController : ControllerBase
{
    private readonly ITopluUrunYonetimServisi _topluUrunServisi;

    public AdminTopluUrunController(ITopluUrunYonetimServisi topluUrunServisi)
    {
        _topluUrunServisi = topluUrunServisi;
    }

    [HttpPost("fiyat-guncelle")]
    public async Task<IActionResult> TopluFiyatGuncelle([FromBody] TopluFiyatGuncelleDto dto)
    {
        await _topluUrunServisi.TopluFiyatGuncelleAsync(dto);
        return Ok(new { Mesaj = "Ürün fiyatları toplu olarak başarıyla güncellendi." });
    }

    [HttpPost("durum-degistir")]
    public async Task<IActionResult> TopluDurumDegistir([FromQuery] bool aktifMi, [FromQuery] Guid? kategoriId = null)
    {
        await _topluUrunServisi.TopluDurumDegistirAsync(aktifMi, kategoriId);
        return Ok(new { Mesaj = $"Ürünlerin durumu başarıyla {(aktifMi ? "satışa açıldı" : "satışa kapatıldı")}." });
    }
}