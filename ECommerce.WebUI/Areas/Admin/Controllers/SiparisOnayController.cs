using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ECommerce.Application.DTOs.SiparisGorselOnay;

namespace ECommerce.WebUI.Controllers
{
    public class SiparisOnayController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SiparisOnayController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("SiparisOnay/Goruntule/{siparisId:guid}")]
        public async Task<IActionResult> Goruntule(Guid siparisId)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"gorsel-onay/{siparisId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var gorselDto = JsonSerializer.Deserialize<SiparisGorselOnayDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(gorselDto);
            }

            return View("Hata", "Bu siparişe ait görüntülenemeyen veya süresi dolmuş bir görsel bulunuyor.");
        }

        [HttpPost("SiparisOnay/OnayVer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnayVer(Guid siparisId)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            // API tarafındaki post endpoint'i body beklemiyor, sadece parametre. 
            var response = await client.PostAsync($"gorsel-onay/{siparisId}/onay-ver", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Çiçek görseli onaylandı. Siparişiniz kısa süre içinde yola çıkacaktır, teşekkür ederiz.";
            }
            else
            {
                TempData["HataMesaji"] = "Onay işlemi sırasında bir hata oluştu. Lütfen tekrar deneyiniz.";
            }

            return RedirectToAction(nameof(Goruntule), new { siparisId = siparisId });
        }
    }
}