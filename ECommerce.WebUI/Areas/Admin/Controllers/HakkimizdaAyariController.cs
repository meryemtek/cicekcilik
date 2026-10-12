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
    [Authorize]
    public class HakkimizdaAyariController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _env;

        public HakkimizdaAyariController(IHttpClientFactory httpClientFactory, IWebHostEnvironment env)
        {
            _httpClientFactory = httpClientFactory;
            _env = env;
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
            // Eğer yeni bir görsel yüklendiyse kaydediyoruz
            if (gorselDosyasi != null && gorselDosyasi.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "hakkimizda");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var benzersizAd = Guid.NewGuid().ToString() + "_" + Path.GetFileName(gorselDosyasi.FileName);
                var dosyaYolu = Path.Combine(uploadsFolder, benzersizAd);

                using (var stream = new FileStream(dosyaYolu, FileMode.Create))
                {
                    await gorselDosyasi.CopyToAsync(stream);
                }

                dto.GorselUrl = $"/uploads/hakkimizda/{benzersizAd}";
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