using ECommerce.Application.DTOs.OneCikan;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers
{
    [Route("api/onecikanlar")]
    [ApiController]
    public class OneCikanlarController : ControllerBase
    {
        private readonly IOneCikanServisi _servis;

        public OneCikanlarController(IOneCikanServisi servis)
        {
            _servis = servis;
        }

        [AllowAnonymous]
        [HttpGet("urunler")]
        public async Task<IActionResult> GetirOneCikanUrunler()
        {
            var urunler = await _servis.OneCikanUrunleriGetirAsync();
            return Ok(urunler);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("ayarlar")]
        public async Task<IActionResult> GetirAyarlar()
        {
            var ayarlar = await _servis.AyarlariGetirAsync();
            return Ok(ayarlar);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("ayarlar")]
        public async Task<IActionResult> GuncelleAyarlar([FromBody] OneCikanDto dto)
        {
            await _servis.AyarlariGuncelleAsync(dto);
            return Ok(new { Mesaj = "Öne çıkan ayarları güncellendi." });
        }
    }
}