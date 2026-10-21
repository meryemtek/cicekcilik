using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Application.Interfaces;

namespace ECommerce.WebAPI.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("api/admin/odeme-bildirimleri")]
    [ApiController]
    public class AdminOdemeBildirimiController : ControllerBase
    {
        private readonly IOdemeBildirimiServisi _servis;
        private readonly IDosyaServisi _dosyaServisi;

        public AdminOdemeBildirimiController(IOdemeBildirimiServisi servis, IDosyaServisi dosyaServisi)
        {
            _servis = servis;
            _dosyaServisi = dosyaServisi;
        }

        [HttpGet]
        public async Task<IActionResult> TumunuGetir()
        {
            var liste = await _servis.TumBildirimleriGetirAsync();
            return Ok(liste);
        }

        [HttpGet("okunmamis-sayi")]
        public async Task<IActionResult> OkunmamisSayisiGetir()
        {
            var sayi = await _servis.OkunmamisBildirimSayisiGetirAsync();
            return Ok(sayi);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> DetayGetir(Guid id)
        {
            var detay = await _servis.BildirimOkuAsync(id);
            if (detay == null) return NotFound("Ödeme bildirimi bulunamadı.");
            return Ok(detay);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Sil(Guid id)
        {
            await _servis.SilAsync(id);
            return Ok(new { Mesaj = "Ödeme bildirimi başarıyla silindi." });
        }

        [HttpGet("{id:guid}/dekont")]
        public async Task<IActionResult> DekontGetir(Guid id)
        {
            var detay = await _servis.BildirimOkuAsync(id);
            if (detay == null || string.IsNullOrEmpty(detay.DekontYolu))
            {
                return NotFound("Dekont bulunamadı.");
            }

            var file = await _dosyaServisi.DosyaOkuAsync(detay.DekontYolu);
            if (file == null)
            {
                return NotFound("Dosya sunucuda bulunamadı.");
            }

            return File(file.Value.Stream, file.Value.ContentType, file.Value.FileName);
        }
    }
}
