using ECommerce.Application.DTOs.Slider;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("api/admin/slider")]
[ApiController]
public class AdminSliderController : ControllerBase
{
    private readonly ISliderServisi _sliderServisi;

    public AdminSliderController(ISliderServisi sliderServisi)
    {
        _sliderServisi = sliderServisi;
    }

    [HttpGet]
    public async Task<IActionResult> TumunuGetir()
    {
        var sliderlar = await _sliderServisi.TumSliderlariGetirAsync();
        return Ok(sliderlar);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Getir(Guid id)
    {
        var slider = await _sliderServisi.IdIleGetirAsync(id);
        if (slider == null) return NotFound("Slider bulunamadı.");
        return Ok(slider);
    }

    [HttpPost]
    public async Task<IActionResult> Ekle([FromBody] SliderEkleDto dto)
    {
        var id = await _sliderServisi.SliderEkleAsync(dto);
        return Ok(new { Id = id, Mesaj = "Slider başarıyla eklendi." });
    }

    [HttpPut]
    public async Task<IActionResult> Guncelle([FromBody] SliderGuncelleDto dto)
    {
        await _sliderServisi.SliderGuncelleAsync(dto);
        return Ok(new { Mesaj = "Slider başarıyla güncellendi." });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Sil(Guid id)
    {
        await _sliderServisi.SliderSilAsync(id);
        return Ok(new { Mesaj = "Slider başarıyla silindi." });
    }
}