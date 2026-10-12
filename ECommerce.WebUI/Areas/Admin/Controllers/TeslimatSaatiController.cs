using ECommerce.Application.DTOs.TeslimatSaati;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ECommerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class TeslimatSaatiController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TeslimatSaatiController(IHttpClientFactory httpClientFactory)
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
            var response = await client.GetAsync("admin/teslimat-saatleri");

            var saatler = new List<TeslimatSaatiDto>();
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                saatler = JsonSerializer.Deserialize<List<TeslimatSaatiDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<TeslimatSaatiDto>();
            }

            return View(saatler);
        }

        [HttpGet]
        public IActionResult Ekle()
        {
            return View(new TeslimatSaatiDto { GunlukKota = 50, AktifMi = true, Sira = 1, TeslimatGrubu = ECommerce.Domain.Enums.TeslimatGrubu.Normal });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(TeslimatSaatiDto dto)
        {
            var client = CreateClientWithToken();
            var jsonContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("admin/teslimat-saatleri", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Teslimat saati başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }

            TempData["HataMesaji"] = "Teslimat saati eklenirken bir hata oluştu.";
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Guncelle(Guid id)
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync($"admin/teslimat-saatleri/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var dto = JsonSerializer.Deserialize<TeslimatSaatiDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guncelle(TeslimatSaatiDto dto)
        {
            var client = CreateClientWithToken();
            var jsonContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

            var response = await client.PutAsync("admin/teslimat-saatleri", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Teslimat saati güncellendi.";
                return RedirectToAction(nameof(Index));
            }

            TempData["HataMesaji"] = "Güncelleme sırasında bir hata oluştu.";
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(Guid id)
        {
            var client = CreateClientWithToken();
            var response = await client.DeleteAsync($"admin/teslimat-saatleri/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Kayıt başarıyla silindi.";
            }
            else
            {
                TempData["HataMesaji"] = "Silme işlemi başarısız oldu.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}