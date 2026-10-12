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
        private readonly IWebHostEnvironment _environment;

        public OdemeBildirimiController(IOdemeBildirimiServisi servis, IWebHostEnvironment environment)
        {
            _servis = servis;
            _environment = environment;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Ekle([FromForm] OdemeBildirimiEkleDto dto)
        {
            var webRootPath = _environment.WebRootPath;
            var id = await _servis.EkleAsync(dto, webRootPath);
            return Ok(new { Id = id, Mesaj = "Ödeme bildiriminiz başarıyla alındı." });
        }
    }
}