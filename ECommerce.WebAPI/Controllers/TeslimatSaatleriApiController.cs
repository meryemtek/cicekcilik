using ECommerce.Application.Services;
using ECommerce.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers
{
    [Route("api/teslimatsaatleri")]
    [ApiController]
    public class TeslimatSaatleriApiController : ControllerBase
    {
        private readonly ITeslimatSaatiServisi _servis;

        public TeslimatSaatleriApiController(ITeslimatSaatiServisi servis)
        {
            _servis = servis;
        }

        [AllowAnonymous]
        [HttpGet("aktif")]
        public async Task<IActionResult> AktifSaatleriGetir()
        {
            var liste = await _servis.AktifSaatleriGetirAsync();
            return Ok(liste);
        }

        [AllowAnonymous]
        [HttpGet("normal")]
        public async Task<IActionResult> NormalSaatleriGetir()
        {
            var liste = await _servis.AktifSaatleriGetirAsync(TeslimatGrubu.Normal);
            return Ok(liste);
        }

        [AllowAnonymous]
        [HttpGet("vip")]
        public async Task<IActionResult> VipSaatleriGetir()
        {
            var liste = await _servis.AktifSaatleriGetirAsync(TeslimatGrubu.Vip);
            return Ok(liste);
        }
    }
}