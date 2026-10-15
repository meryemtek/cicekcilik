using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Application.DTOs.Hakkimizda;
using ECommerce.Application.Interfaces;

namespace ECommerce.WebAPI.Controllers.Admin
{
    [Authorize(Roles = "Admin")] // Sadece giriş yapmış adminler
    [Route("api/admin/hakkimizda")]
    [ApiController]
    public class HakkimizdaAyarlariController : ControllerBase
    {
        private readonly IHakkimizdaServisi _hakkimizdaServisi;

        public HakkimizdaAyarlariController(IHakkimizdaServisi hakkimizdaServisi)
        {
            _hakkimizdaServisi = hakkimizdaServisi;
        }

        [HttpGet]
        public async Task<IActionResult> Getir()
        {
            var ayarlar = await _hakkimizdaServisi.GetirAsync();
            return Ok(ayarlar);
        }

        [HttpPut]
        public async Task<IActionResult> Guncelle([FromBody] HakkimizdaAyariDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _hakkimizdaServisi.GuncelleAsync(dto);
            return Ok(new { Mesaj = "Hakkımızda sayfası başarıyla güncellendi." });
        }
    }
}