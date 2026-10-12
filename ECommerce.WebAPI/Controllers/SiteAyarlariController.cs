using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers
{
    [Route("api/siteayarlari")]
    [ApiController]
    public class SiteAyarlariController : ControllerBase
    {
        private readonly ISiteAyariServisi _siteAyariServisi;

        public SiteAyarlariController(ISiteAyariServisi siteAyariServisi)
        {
            _siteAyariServisi = siteAyariServisi;
        }

        [HttpGet("ozet")]
        public async Task<IActionResult> Getir()
        {
            var ayarlar = await _siteAyariServisi.AyarlariGetirAsync();

            // Güvenlik gereği sadece giriş sayfasında lazım olan vitrin bilgilerini dönüyoruz
            return Ok(new
            {
                FirmaAdi = ayarlar.FirmaAdi,
                LogoUrl = ayarlar.LogoUrl,
                TemaRengiHex = ayarlar.TemaRengiHex
            });
        }

        [AllowAnonymous]
        [HttpGet("site-ayarlari")]
        public async Task<IActionResult> SiteAyarlariniGetir()
        {
            var ayarlar = await _siteAyariServisi.AyarlariGetirAsync();
            return Ok(ayarlar);
        }
    }
}