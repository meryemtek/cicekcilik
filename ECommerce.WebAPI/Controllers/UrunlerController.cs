using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Application.DTOs.Urun;
using ECommerce.Application.Interfaces;
using ECommerce.Application.DTOs.UrunYorum;

namespace ECommerce.WebAPI.Controllers
{
    [Route("api/urunler")]
    [ApiController]
    public class UrunlerController : ControllerBase
    {
        private readonly IUrunServisi _urunServisi;

        public UrunlerController(IUrunServisi urunServisi)
        {
            _urunServisi = urunServisi;
        }

        [HttpGet("{seoUrl}")]
        [AllowAnonymous]
        public async Task<IActionResult> DetayGetir(string seoUrl)
        {
            var urun = await _urunServisi.SeoUrlIleDetayGetirAsync(seoUrl);
            if (urun == null) return NotFound("Ürün bulunamadı.");
            return Ok(urun);
        }

        [HttpPost("yorum-ekle")]
        [AllowAnonymous]
        public async Task<IActionResult> YorumEkle([FromBody] UrunYorumEkleDto dto)
        {
            await _urunServisi.UrunYorumuEkleAsync(dto);
            return Ok(new { Mesaj = "Yorumunuz başarıyla alındı." });
        }

        [HttpGet("kategori/{kategoriId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> KategoriyeGoreGetir(Guid kategoriId)
        {
            var urunler = await _urunServisi.KategoriIdyeGoreGetirAsync(kategoriId);
            return Ok(urunler);
        }

        [HttpGet("id-ile/{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> IdIleDetayGetir(Guid id)
        {
            // DİKKAT: IdIleGetirAsync yerine IdIleDetayGetirAsync çağrılıyor (Varyantların gelmesi için)
            var urun = await _urunServisi.IdIleDetayGetirAsync(id);
            if (urun == null) return NotFound("Ürün bulunamadı.");
            return Ok(urun);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> TumunuGetir()
        {
            var urunler = await _urunServisi.TumUrunleriGetirAsync();
            var aktifUrunler = urunler.Where(x => x.AktifMi && x.StokMiktari > 0).ToList();
            return Ok(aktifUrunler);
        }
    }
}