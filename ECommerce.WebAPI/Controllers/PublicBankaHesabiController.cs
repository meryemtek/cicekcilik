using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Application.Interfaces;

namespace ECommerce.WebAPI.Controllers
{
    [Route("api/banka-hesaplari")]
    [ApiController]
    public class PublicBankaHesabiController : ControllerBase
    {
        private readonly IBankaHesabiServisi _bankaHesabiServisi;

        public PublicBankaHesabiController(IBankaHesabiServisi bankaHesabiServisi)
        {
            _bankaHesabiServisi = bankaHesabiServisi;
        }

        [HttpGet("aktif")]
        [AllowAnonymous]
        public async Task<IActionResult> AktifHesaplariGetir()
        {
            var hesaplar = await _bankaHesabiServisi.AktifBankaHesaplariniGetirAsync();
            return Ok(hesaplar);
        }
    }
}