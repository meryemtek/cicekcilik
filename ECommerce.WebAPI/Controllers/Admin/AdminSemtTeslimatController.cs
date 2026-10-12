using ECommerce.Application.DTOs.SemtTeslimatBolgesi;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers.Admin
{
    [Authorize]
    [Route("api/admin/semt-teslimat")]
    [ApiController]
    public class AdminSemtTeslimatController : ControllerBase 
    {
        private readonly ISemtTeslimatServisi _semtTeslimatServisi;

        public AdminSemtTeslimatController(ISemtTeslimatServisi semtTeslimatServisi)
        {
            _semtTeslimatServisi = semtTeslimatServisi;
        }

        [HttpGet]
        public async Task<IActionResult> TumunuGetir()
        {
            var bolgeler = await _semtTeslimatServisi.TumBolgeleriGetirAsync();
            return Ok(bolgeler);
        }

        // YENİ EKLENEN SAYFALAMA ENDPOINT'İ
        [HttpGet("sayfali")]
        public async Task<IActionResult> SayfaliGetir([FromQuery] int start, [FromQuery] int length, [FromQuery] string aramaDegeri = "")
        {
            var sonuc = await _semtTeslimatServisi.SayfaliGetirAsync(start, length, aramaDegeri ?? "");
            return Ok(sonuc);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Getir(Guid id)
        {
            var bolge = await _semtTeslimatServisi.IdIleGetirAsync(id);
            if (bolge == null) return NotFound("Bölge bulunamadı.");
            return Ok(bolge);
        }

        [HttpPost]
        public async Task<IActionResult> Ekle([FromBody] SemtBolgeEkleDto dto)
        {
            var id = await _semtTeslimatServisi.EkleAsync(dto);
            return Ok(new { Id = id, Mesaj = "Bölgesel teslimat ücreti başarıyla eklendi." });
        }

        [HttpPut]
        public async Task<IActionResult> Guncelle([FromBody] SemtBolgeGuncelleDto dto)
        {
            await _semtTeslimatServisi.GuncelleAsync(dto);
            return Ok(new { Mesaj = "Bölgesel teslimat ücreti başarıyla güncellendi." });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Sil(Guid id)
        {
            await _semtTeslimatServisi.SilAsync(id);
            return Ok(new { Mesaj = "Bölgesel teslimat ücreti başarıyla silindi." });
        }
    }
}