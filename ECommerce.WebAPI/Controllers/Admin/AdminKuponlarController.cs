using ECommerce.Application.DTOs.Kupon;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("api/admin/kuponlar")]
[ApiController]
public class AdminKuponlarController : ControllerBase
{
    private readonly IKuponServisi _kuponServisi;

    public AdminKuponlarController(IKuponServisi kuponServisi)
    {
        _kuponServisi = kuponServisi;
    }

    [HttpGet]
    public async Task<IActionResult> TumunuGetir()
    {
        var kuponlar = await _kuponServisi.TumKuponlariGetirAsync();
        return Ok(kuponlar);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Getir(Guid id)
    {
        var kupon = await _kuponServisi.IdIleGetirAsync(id);
        if (kupon == null) return NotFound("İndirim kuponu bulunamadı.");
        return Ok(kupon);
    }

    [HttpPost]
    public async Task<IActionResult> Ekle([FromBody] KuponEkleDto dto)
    {
        var id = await _kuponServisi.EkleAsync(dto);
        return Ok(new { Id = id, Mesaj = "İndirim kuponu başarıyla eklendi." });
    }

    [HttpPut]
    public async Task<IActionResult> Guncelle([FromBody] KuponGuncelleDto dto)
    {
        await _kuponServisi.GuncelleAsync(dto);
        return Ok(new { Mesaj = "İndirim kuponu başarıyla güncellendi." });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Sil(Guid id)
    {
        await _kuponServisi.SilAsync(id);
        return Ok(new { Mesaj = "İndirim kuponu başarıyla silindi." });
    }
}