using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using ECommerce.Application.DTOs.Hakkimizda;

namespace ECommerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HakkimizdaAyariController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HakkimizdaAyariController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateClientWithToken()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var token = User.FindFirst("JWToken")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync("admin/hakkimizda");
            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadFromJsonAsync<HakkimizdaAyariDto>();
                return View(dto ?? new HakkimizdaAyariDto());
            }
            return View(new HakkimizdaAyariDto());
        }

        [HttpGet]
        public async Task<IActionResult> Duzenle()
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync("admin/hakkimizda");
            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadFromJsonAsync<HakkimizdaAyariDto>();
                return View(dto ?? new HakkimizdaAyariDto());
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duzenle(HakkimizdaAyariDto dto, IFormFile? gorselDosyasi)
        {
            if (gorselDosyasi != null && gorselDosyasi.Length > 0)
            {
                dto.GorselUrl = await ECommerce.WebUI.Services.ApiGorselYukleyici.YukleAsync(
                    _httpClientFactory, User, gorselDosyasi, "hakkimizda");
            }

            var client = CreateClientWithToken();
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var jsonContent = new StringContent(JsonSerializer.Serialize(dto, jsonOptions), Encoding.UTF8, "application/json");

            var response = await client.PutAsync("admin/hakkimizda", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Hakkımızda sayfası başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }

            TempData["HataMesaji"] = "Güncelleme sırasında bir hata oluştu.";
            return View(dto);
        }
    }
}
