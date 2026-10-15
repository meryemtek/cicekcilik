using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("api/admin/adres-havuzu")]
    [ApiController]
    public class AdminAdresHavuzuController : ControllerBase
    {
        private readonly IAdresHavuzuServisi _adresHavuzuServisi;

        public AdminAdresHavuzuController(IAdresHavuzuServisi adresHavuzuServisi)
        {
            _adresHavuzuServisi = adresHavuzuServisi;
        }

        [HttpGet("iller")]
        public IActionResult IlleriGetir()
        {
            return Ok(_adresHavuzuServisi.IlleriGetir());
        }

        [HttpGet("ilceler")]
        public IActionResult IlceleriGetir([FromQuery] string ilAdi)
        {
            return Ok(_adresHavuzuServisi.IlceleriGetir(ilAdi));
        }

        [HttpGet("mahalleler")]
        public IActionResult MahalleleriGetir([FromQuery] string ilAdi, [FromQuery] string ilceAdi)
        {
            return Ok(_adresHavuzuServisi.MahalleleriGetir(ilAdi, ilceAdi));
        }
    }
}