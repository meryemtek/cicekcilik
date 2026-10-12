using ECommerce.Application.DTOs.Favori;
using Microsoft.AspNetCore.Mvc;

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
            var favoriler = await _favoriServisi.KullaniciFavorileriniGetirAsync(kullaniciId);
            return Ok(favoriler);
        }

        [HttpPost("toggle")]
        public async Task<IActionResult> Toggle([FromBody] FavoriEkleDto dto)
        {
            var sonuc = await _favoriServisi.FavoriToggleAsync(dto.KullaniciId, dto.UrunId);
            return Ok(new { EklendiMi = sonuc });
        }
    }
}
