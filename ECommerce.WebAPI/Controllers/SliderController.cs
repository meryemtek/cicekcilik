using ECommerce.Application.DTOs.Slider;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebAPI.Controllers
{
    [Route("api/slider")]
    [ApiController]
    public class SliderController : ControllerBase
    {
        private readonly ISliderServisi _sliderServisi;

        public SliderController(ISliderServisi sliderServisi)
        {
            _sliderServisi = sliderServisi;
        }

        [AllowAnonymous]
        [HttpGet("aktif")]
        public async Task<IActionResult> AktifSliderlariGetir()
        {
            var sliderlar = await _sliderServisi.AktifSliderlariGetirAsync();

            return Ok(sliderlar);
        }
    }
}