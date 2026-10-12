using ECommerce.Application.DTOs.Blog;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers
{
    [Route("api/blog")]
    [ApiController]
    public class BlogApiController : ControllerBase
    {
        private readonly IBlogServisi _blogServisi;

        public BlogApiController(IBlogServisi blogServisi)
        {
            _blogServisi = blogServisi;
        }

        // Ön Yüz: Ana sayfada veya blog listeleme sayfasında sadece Aktif olanları getirmek için
        [HttpGet("aktif-liste")]
        public async Task<IActionResult> AktifYazilariGetir()
        {
            var yazilar = await _blogServisi.TumYazilariGetirAsync();
            var aktifYazilar = yazilar.Where(x => x.AktifMi).ToList();

            return Ok(aktifYazilar);
        }

        // Ön Yüz: Blog detay sayfası için SeoUrl ile veriyi getiren uç
        [HttpGet("detay/{seoUrl}")]
        public async Task<IActionResult> DetayGetir(string seoUrl)
        {
            var yazi = await _blogServisi.SeoUrlIleGetirAsync(seoUrl);

            if (yazi == null)
            {
                return NotFound("Aradığınız blog yazısı bulunamadı veya yayından kaldırılmış olabilir.");
            }

            return Ok(yazi);
        }
    }
}