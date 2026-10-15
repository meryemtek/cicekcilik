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
        private readonly IWebHostEnvironment _environment; // 1. Alan tanımlandı

        public AdminOdemeBildirimiController(IOdemeBildirimiServisi servis, IWebHostEnvironment environment) // 2. Parametre eklendi
        {
            _servis = servis;
            _environment = environment; // 3. Ataması yapıldı
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

            var filePath = Path.Combine(_environment.WebRootPath, detay.DekontYolu.TrimStart('/', '\\'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Dosya sunucuda bulunamadı.");
            }

            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, contentType, Path.GetFileName(filePath));
        }
    }
}