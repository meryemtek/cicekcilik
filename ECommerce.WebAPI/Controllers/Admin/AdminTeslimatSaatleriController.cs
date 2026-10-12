using ECommerce.Application.DTOs.TeslimatSaati;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers.Admin
{
    [Authorize] // Admin yetkilendirmesi
    [Route("api/admin/teslimat-saatleri")]
    [ApiController]
    public class AdminTeslimatSaatleriController : ControllerBase
    {
        private readonly ITeslimatSaatiServisi _teslimatSaatiServisi;

        public AdminTeslimatSaatleriController(ITeslimatSaatiServisi teslimatSaatiServisi)
        {
            _teslimatSaatiServisi = teslimatSaatiServisi;
        }

        [HttpGet]
        public async Task<IActionResult> TumunuGetir()
        {
            var saatler = await _teslimatSaatiServisi.TumunuGetirAsync();
            return Ok(saatler);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetirById(Guid id)
        {
            var saat = await _teslimatSaatiServisi.GetirByIdAsync(id);
            if (saat == null)
            {
                return NotFound(new { Hata = "Teslimat saati bulunamadı." });
            }
            return Ok(saat);
        }

        [HttpPost]
        public async Task<IActionResult> Ekle([FromBody] TeslimatSaatiDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _teslimatSaatiServisi.EkleAsync(dto);
            return Ok(new { Mesaj = "Teslimat saati başarıyla eklendi." });
        }

        [HttpPut]
        public async Task<IActionResult> Guncelle([FromBody] TeslimatSaatiDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mevcut = await _teslimatSaatiServisi.GetirByIdAsync(dto.Id);
            if (mevcut == null)
            {
                return NotFound(new { Hata = "Güncellenecek kayıt bulunamadı." });
            }

            await _teslimatSaatiServisi.GuncelleAsync(dto);
            return Ok(new { Mesaj = "Teslimat saati başarıyla güncellendi." });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Sil(Guid id)
        {
            var mevcut = await _teslimatSaatiServisi.GetirByIdAsync(id);
            if (mevcut == null)
            {
                return NotFound(new { Hata = "Silinecek kayıt bulunamadı." });
            }

            await _teslimatSaatiServisi.SilAsync(id);
            return Ok(new { Mesaj = "Teslimat saati başarıyla silindi." });
        }
    }
}