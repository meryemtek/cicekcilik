using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Application.DTOs.OdemeBildirimi;
using ECommerce.Application.Interfaces;

namespace ECommerce.WebAPI.Controllers
{
    [Route("api/odeme-bildirimi")]
    [ApiController]
    public class OdemeBildirimiController : ControllerBase
    {
        private readonly IOdemeBildirimiServisi _servis;

        public OdemeBildirimiController(IOdemeBildirimiServisi servis)
        {
            _servis = servis;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Ekle([FromForm] OdemeBildirimiEkleDto dto)
        {
            var id = await _servis.EkleAsync(dto);
            return Ok(new { Id = id, Mesaj = "Ödeme bildiriminiz başarıyla alındı." });
        }
    }
}
