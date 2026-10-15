using ECommerce.Application.DTOs.Blog;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("api/admin/blog")]
[ApiController]
public class AdminBlogController : ControllerBase
{
    private readonly IBlogServisi _blogServisi;

    public AdminBlogController(IBlogServisi blogServisi)
    {
        _blogServisi = blogServisi;
    }

    [HttpGet]
    public async Task<IActionResult> TumunuGetir()
    {
        var yazilar = await _blogServisi.TumYazilariGetirAsync();
        return Ok(yazilar);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Getir(Guid id)
    {
        var yazi = await _blogServisi.IdIleGetirAsync(id);
        if (yazi == null) return NotFound("Blog yazısı bulunamadı.");
        return Ok(yazi);
    }

    [HttpPost]
    public async Task<IActionResult> Ekle([FromBody] BlogEkleDto dto)
    {
        var id = await _blogServisi.EkleAsync(dto);
        return Ok(new { Id = id, Mesaj = "Blog yazısı başarıyla eklendi." });
    }

    [HttpPut]
    public async Task<IActionResult> Guncelle([FromBody] BlogGuncelleDto dto)
    {
        await _blogServisi.GuncelleAsync(dto);
        return Ok(new { Mesaj = "Blog yazısı başarıyla güncellendi." });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Sil(Guid id)
    {
        await _blogServisi.SilAsync(id);
        return Ok(new { Mesaj = "Blog yazısı başarıyla silindi." });
    }
}