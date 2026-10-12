using ECommerce.Application.DTOs.Sayfa; 
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;

namespace ECommerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SayfaController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SayfaController(IHttpClientFactory httpClientFactory)
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
            var response = await client.GetAsync("admin/sayfalar");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var sayfalar = JsonSerializer.Deserialize<List<SayfaListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(sayfalar ?? new List<SayfaListeDto>());
            }

            return View(new List<SayfaListeDto>());
        }

        [HttpGet]
        public IActionResult Ekle()
        {
            return View(new SayfaEkleDto { AktifMi = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(SayfaEkleDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var client = CreateClientWithToken();
            var jsonContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("admin/sayfalar", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Sayfa başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }

            TempData["HataMesaji"] = "Sayfa eklenirken bir hata oluştu.";
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Guncelle(Guid id)
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync($"admin/sayfalar/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var sayfa = JsonSerializer.Deserialize<SayfaGuncelleDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (sayfa != null)
                {
                    return View(sayfa);
                }
            }

            TempData["HataMesaji"] = "Güncellenmek istenen sayfa bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guncelle(SayfaGuncelleDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var client = CreateClientWithToken();
            var jsonContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

            var response = await client.PutAsync("admin/sayfalar", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Sayfa başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }

            TempData["HataMesaji"] = "Sayfa güncellenirken bir hata oluştu.";
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(Guid id)
        {
            var client = CreateClientWithToken();
            var response = await client.DeleteAsync($"admin/sayfalar/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Sayfa başarıyla silindi.";
            }
            else
            {
                TempData["HataMesaji"] = "Sayfa silinirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}