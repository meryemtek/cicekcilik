using ECommerce.Application.DTOs.Siparis;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SiparisController : ControllerBase
    {
        private readonly ISiparisServisi _siparisServisi;

        public SiparisController(ISiparisServisi siparisServisi)
        {
            _siparisServisi = siparisServisi;
        }

        [Authorize]
        [HttpGet("benim-siparislerim")]
        public async Task<IActionResult> BenimSiparislerim()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var kullaniciId))
            {
                return Unauthorized(new { mesaj = "Geçersiz oturum bilgisi." });
            }

            var siparisler = await _siparisServisi.KullanicininSiparisleriniGetirAsync(kullaniciId);
            return Ok(siparisler);
        }

        [Authorize]
        [HttpGet("benim-siparislerim/{id:guid}")]
        public async Task<IActionResult> BenimSiparisDetayim(Guid id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdString, out var kullaniciId)) return Unauthorized();
            var siparisler = await _siparisServisi.KullanicininSiparisleriniGetirAsync(kullaniciId);
            if (!siparisler.Any(s => s.Id == id)) return NotFound();
            var detay = await _siparisServisi.SiparisDetayGetirAsync(id);
            return detay == null ? NotFound() : Ok(detay);
        }

        [HttpPost("olustur")]
        public async Task<IActionResult> SiparisOlustur([FromBody] SiparisOlusturDto dto)
        {
            // Eğer kullanıcı sisteme giriş yapmışsa, Token'dan KullaniciId'yi otomatik set edelim
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var kullaniciId))
            {
                dto.KullaniciId = kullaniciId;
            }
            else
            {
                dto.KullaniciId = null; // Misafir siparişi
            }

            try
            {
                var siparisId = await _siparisServisi.SiparisOlusturAsync(dto);
                return Ok(new { siparisId, mesaj = "Sipariş başarıyla oluşturuldu." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mesaj = ex.Message });
            }
        }

        [HttpPost("{id:guid}/odeme-baslatma-iptal")]
        public async Task<IActionResult> OdemeBaslatmaIptaliniIsle(Guid id)
        {
            try
            {
                await _siparisServisi.OdemeBaslatmaIptaliniIsleAsync(id);
                return Ok(new { mesaj = "Ödeme başlatma işlemi iptal edildi." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { mesaj = "Sipariş bulunamadı." });
            }
        }
    }
}
