using ECommerce.Application.DTOs.Favori;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.WebAPI.Controllers
{
    [Route("api/favori")]
    [ApiController]
    public class FavoriController : ControllerBase
    {
        private readonly IFavoriServisi _favoriServisi;

        public FavoriController(IFavoriServisi favoriServisi)
        {
            _favoriServisi = favoriServisi;
        }

        [HttpGet("{kullaniciId}")]
        public async Task<IActionResult> Getir(string kullaniciId)
        {
            var anahtar = FavoriAnahtari(kullaniciId);
            if (anahtar == null) return BadRequest(new { mesaj = "Geçersiz ziyaretçi kimliği." });
            var favoriler = await _favoriServisi.KullaniciFavorileriniGetirAsync(anahtar);
            return Ok(favoriler);
        }

        [HttpPost("toggle")]
        public async Task<IActionResult> Toggle([FromBody] FavoriEkleDto dto)
        {
            if (dto.UrunId == Guid.Empty) return BadRequest();
            var anahtar = FavoriAnahtari(dto.KullaniciId);
            if (anahtar == null) return BadRequest(new { mesaj = "Geçersiz ziyaretçi kimliği." });
            var sonuc = await _favoriServisi.FavoriToggleAsync(anahtar, dto.UrunId);
            return Ok(new { EklendiMi = sonuc });
        }

        private string? FavoriAnahtari(string? ziyaretciId)
        {
            if (User.Identity?.IsAuthenticated == true &&
                Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var kullaniciId))
                return $"user:{kullaniciId:D}";

            return Guid.TryParse(ziyaretciId, out var id) ? id.ToString("D") : null;
        }
    }
}
