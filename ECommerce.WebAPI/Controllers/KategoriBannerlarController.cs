using ECommerce.Application.DTOs.KategoriBanner;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers
{
    [Route("api/kategori-bannerlar")]
    [ApiController]
    public class KategoriBannerlarController : ControllerBase
    {
        private readonly IKategoriBannerServisi _servis;

        public KategoriBannerlarController(IKategoriBannerServisi servis)
        {
            _servis = servis;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Getir()
        {
            var liste = await _servis.TumunuGetirAsync();
            return Ok(liste);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetirById(Guid id)
        {
            var model = await _servis.GetirByIdAsync(id);
            return Ok(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Ekle([FromBody] KategoriBannerEkleGuncelleDto dto)
        {
            await _servis.EkleAsync(dto);
            return Ok(new { Mesaj = "Başarıyla eklendi." });
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Guncelle([FromBody] KategoriBannerEkleGuncelleDto dto)
        {
            await _servis.GuncelleAsync(dto);
            return Ok(new { Mesaj = "Başarıyla güncellendi." });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Sil(Guid id)
        {
            await _servis.SilAsync(id);
            return Ok(new { Mesaj = "Başarıyla silindi." });
        }
    }
}