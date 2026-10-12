using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Application.DTOs.Menu; // Projedeki DTO namespace'ine göre kontrol edin
using ECommerce.Application.Interfaces;
using ECommerce.Application.DTOs.IletisimMesaji;

namespace ECommerce.WebAPI.Controllers
{
    [Route("api/iletisim")]
    [ApiController]
    public class IletisimController : ControllerBase
    {
        private readonly IIletisimMesajiServisi _iletisimMesajiServisi;

        public IletisimController(IIletisimMesajiServisi iletisimMesajiServisi)
        {
            _iletisimMesajiServisi = iletisimMesajiServisi;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> MesajGonder([FromBody] IletisimMesajiEkleDto dto)
        {
            var id = await _iletisimMesajiServisi.EkleAsync(dto);
            return Ok(new { Id = id, Mesaj = "Mesajınız başarıyla alındı." });
        }
    }
}