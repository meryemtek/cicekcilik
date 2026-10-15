using ECommerce.Application.DTOs.AnasayfaKategori;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers
{
    [Route("api/anasayfa-kategoriler")]
    [ApiController]
    public class AnasayfaKategorilerController : ControllerBase
    {
        private readonly IAnasayfaKategoriServisi _servis;

        public AnasayfaKategorilerController(
            IAnasayfaKategoriServisi servis)
        {
            _servis = servis;
        }

        [AllowAnonymous]
        [HttpGet("aktif")]
        public async Task<IActionResult> AktifleriGetir()
        {
            var liste = await _servis.AktifleriGetirAsync();

            return Ok(liste);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> TumunuGetir()
        {
            var liste = await _servis.TumunuGetirAsync();

            return Ok(liste);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Getir(Guid id)
        {
            var item = await _servis.IdIleGetirAsync(id);

            if (item == null)
                return NotFound("Anasayfa kategorisi bulunamadı.");

            return Ok(item);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Ekle(
            [FromBody] AnasayfaKategoriEkleDto dto)
        {
            try
            {
                var id = await _servis.EkleAsync(dto);

                return Ok(new
                {
                    Id = id,
                    Mesaj = "Anasayfa kategorisi başarıyla eklendi."
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    Mesaj = ex.Message
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> Guncelle(
            [FromBody] AnasayfaKategoriGuncelleDto dto)
        {
            try
            {
                await _servis.GuncelleAsync(dto);

                return Ok(new
                {
                    Mesaj = "Anasayfa kategorisi başarıyla güncellendi."
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    Mesaj = ex.Message
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Sil(Guid id)
        {
            await _servis.SilAsync(id);

            return Ok(new
            {
                Mesaj = "Anasayfa kategorisi kaldırıldı."
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("sirala")]
        public async Task<IActionResult> Sirala([FromBody] List<Guid> idler)
        {
            if (idler == null || !idler.Any())
                return BadRequest(new { Mesaj = "Geçersiz sıralama verisi." });

            await _servis.SiralariGuncelleAsync(idler);

            return Ok(new
            {
                Mesaj = "Kategori sıralaması güncellendi."
            });
        }
    }
}