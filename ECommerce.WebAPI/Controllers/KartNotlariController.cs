using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/kart-notlari")]
    [ApiController]
    public class KartNotlariController : ControllerBase
    {
        private readonly IKartNotuSablonuServisi _kartNotuServisi;

        public KartNotlariController(IKartNotuSablonuServisi kartNotuServisi)
        {
            _kartNotuServisi = kartNotuServisi;
        }

        [HttpGet("aktif")]
        public async Task<IActionResult> AktifKartNotlariniGetir()
        {
            var sablonlar = await _kartNotuServisi.TumSablonlariGetirAsync();
            var aktifSablonlar = sablonlar
                .Where(x => x.AktifMi)
                .OrderBy(x => x.Kategori)
                .ThenBy(x => x.SiraNo)
                .ToList();

            return Ok(aktifSablonlar);
        }
    }
}