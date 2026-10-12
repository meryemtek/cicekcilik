using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Application.Interfaces;

namespace ECommerce.WebAPI.Controllers
{
    [AllowAnonymous] // Herkese açık
    [Route("api/hakkimizda")]
    [ApiController]
    public class HakkimizdaController : ControllerBase
    {
        private readonly IHakkimizdaServisi _hakkimizdaServisi;

        public HakkimizdaController(IHakkimizdaServisi hakkimizdaServisi)
        {
            _hakkimizdaServisi = hakkimizdaServisi;
        }

        [HttpGet]
        public async Task<IActionResult> Getir()
        {
            // Kullanıcıya sadece aktif olan bölümleri filtreleyip de dönebilirsin 
            // ama şimdilik DTO'nun tamamını dönüyoruz, ön yüzde if kontrolleriyle halledeceğiz.
            var ayarlar = await _hakkimizdaServisi.GetirAsync();
            return Ok(ayarlar);
        }
    }
}