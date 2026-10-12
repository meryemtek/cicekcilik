using ECommerce.Application.DTOs.Sayfa;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebUI.Controllers
{
    public class SayfaSiteController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SayfaSiteController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [Route("sayfa/{sayfaKodu}")]
        public async Task<IActionResult> Detay(string sayfaKodu)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            // WebAPI üzerindeki SayfaApiController'a istek atıyoruz
            var response = await client.GetAsync($"sayfalar/{sayfaKodu}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var sayfa = await response.Content.ReadFromJsonAsync<SayfaGuncelleDto>();

            if (sayfa == null || !sayfa.AktifMi) return NotFound();

            return View(sayfa);
        }
    }
}