using ECommerce.Application.DTOs.BankaHesabi;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("api/admin/banka-hesaplari")]
[ApiController]
public class AdminBankaHesaplariController : ControllerBase
{
    private readonly IBankaHesabiServisi _bankaHesabiServisi;

    public AdminBankaHesaplariController(IBankaHesabiServisi bankaHesabiServisi)
    {
        _bankaHesabiServisi = bankaHesabiServisi;
    }

    [HttpGet]
    public async Task<IActionResult> TumunuGetir()
    {
        var hesaplar = await _bankaHesabiServisi.TumBankaHesaplariniGetirAsync();
        return Ok(hesaplar);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Getir(Guid id)
    {
        var hesap = await _bankaHesabiServisi.IdIleGetirAsync(id);
        if (hesap == null) return NotFound("Banka hesabı bulunamadı.");
        return Ok(hesap);
    }

    [HttpPost]
    public async Task<IActionResult> Ekle([FromBody] BankaHesabiEkleDto dto)
    {
        var id = await _bankaHesabiServisi.EkleAsync(dto);
        return Ok(new { Id = id, Mesaj = "Banka hesabı başarıyla eklendi." });
    }

    [HttpPut]
    public async Task<IActionResult> Guncelle([FromBody] BankaHesabiGuncelleDto dto)
    {
        await _bankaHesabiServisi.GuncelleAsync(dto);
        return Ok(new { Mesaj = "Banka hesabı başarıyla güncellendi." });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Sil(Guid id)
    {
        await _bankaHesabiServisi.SilAsync(id);
        return Ok(new { Mesaj = "Banka hesabı başarıyla silindi." });
    }
}