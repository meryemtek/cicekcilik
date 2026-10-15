using ECommerce.Application.DTOs.SiteAyari;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("api/admin/site-ayarlari")]
    [ApiController]
    public class AdminSiteAyarlariController : ControllerBase
    {
        private readonly ISiteAyariServisi _siteAyariServisi;

        public AdminSiteAyarlariController(ISiteAyariServisi siteAyariServisi)
        {
            _siteAyariServisi = siteAyariServisi;
        }

        [HttpGet]
        public async Task<IActionResult> Getir()
        {
            var ayarlar = await _siteAyariServisi.AyarlariGetirAsync();
            return Ok(ayarlar);
        }

        [HttpPut]
        public async Task<IActionResult> Guncelle([FromBody] SiteAyariDto dto)
        {
            if (!ModelState.IsValid)
            {
                var hatalar = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                Console.WriteLine("MODEL VALIDATION HATASI: " + hatalar);
                return BadRequest(new { Hata = hatalar });
            }

            await _siteAyariServisi.AyarlariGuncelleAsync(dto);
            return Ok(new { Mesaj = "Site ayarları başarıyla güncellendi." });
        }
    }
}