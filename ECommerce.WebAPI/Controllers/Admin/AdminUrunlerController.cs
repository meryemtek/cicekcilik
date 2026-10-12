using ECommerce.Application.DTOs.Urun;
using ECommerce.Application.DTOs.UrunYorum;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers.Admin;

[Authorize]
[Route("api/admin/urunler")]
[ApiController]
public class AdminUrunlerController : ControllerBase
{
    private readonly IUrunServisi _urunServisi;

    public AdminUrunlerController(IUrunServisi urunServisi)
    {
        _urunServisi = urunServisi;
    }

    [HttpGet]
    public async Task<IActionResult> TumunuGetir()
    {
        var urunler = await _urunServisi.TumUrunleriGetirAsync();
        return Ok(urunler);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Getir(Guid id)
    {
        var urun = await _urunServisi.IdIleGetirAsync(id);
        if (urun == null) return NotFound("Ürün bulunamadı.");
        return Ok(urun);
    }

    [HttpPost]
    public async Task<IActionResult> Ekle([FromBody] UrunEkleDto dto)
    {
        var id = await _urunServisi.EkleAsync(dto);
        return Ok(new { Id = id, Mesaj = "Ürün başarıyla eklendi." });
    }

    [HttpPut]
    public async Task<IActionResult> Guncelle([FromBody] UrunGuncelleDto dto)
    {
        await _urunServisi.GuncelleAsync(dto);
        return Ok(new { Mesaj = "Ürün başarıyla güncellendi." });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Sil(Guid id)
    {
        await _urunServisi.SilAsync(id);
        return Ok(new { Mesaj = "Ürün başarıyla silindi." });
    }

    [HttpPost("ai-aciklama-uret")]
    public async Task<IActionResult> AiAciklamaUret([FromBody] AiAciklamaIstekDto dto, [FromServices] IAiServisi aiServisi)
    {
        var aciklama = await aiServisi.UrunAciklamasiUretAsync(dto.UrunAdi, dto.KategoriAdi);
        return Ok(new { Aciklama = aciklama });
    }

    [HttpPut("durum-degistir/{id:guid}")]
    public async Task<IActionResult> DurumDegistir(Guid id, [FromBody] UrunDurumGuncelleDto dto)
    {
        await _urunServisi.DurumDegistirAsync(id, dto.AktifMi);
        return Ok(new { Mesaj = "Ürün durumu başarıyla güncellendi." });
    }
}