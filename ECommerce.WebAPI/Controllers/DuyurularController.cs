using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ECommerce.Application.Interfaces;

namespace ECommerce.WebAPI.Controllers
{
    [Route("api/duyurular")]
    [ApiController]
    public class DuyurularController : ControllerBase
    {
        private readonly IDuyuruServisi _duyuruServisi;

        public DuyurularController(IDuyuruServisi duyuruServisi)
        {
            _duyuruServisi = duyuruServisi;
        }

        // Ön yüzde kayan bant için çağrılan adres: https://localhost:7192/api/duyurular/aktif
        [AllowAnonymous]
        [HttpGet("aktif")]
        public async Task<IActionResult> AktifDuyurulariGetir()
        {
            var duyurular = await _duyuruServisi.AktifDuyurulariGetirAsync();
            return Ok(duyurular);
        }
    }
}