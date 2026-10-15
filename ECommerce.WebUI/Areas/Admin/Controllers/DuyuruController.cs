using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ECommerce.Application.DTOs.Duyuru;

namespace ECommerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DuyuruController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DuyuruController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateApiClientWithToken()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            var token = User.FindFirst("JWToken")?.Value;

            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        // 1. Liste
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = CreateApiClientWithToken();

            var response = await client.GetAsync("admin/duyurular");

            if (!response.IsSuccessStatusCode)
            {
                var hata = await response.Content.ReadAsStringAsync();

                return Content(
                    $"API Hatası: {(int)response.StatusCode} {response.StatusCode}\n\n{hata}");
            }

            var liste = await response.Content
                .ReadFromJsonAsync<List<DuyuruDto>>();

            return View(liste ?? new List<DuyuruDto>());
        }

        // 2. Ekleme Sayfası
        [HttpGet]
        public IActionResult Ekle()
        {
            return View();
        }

        // 3. Ekleme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(DuyuruEkleDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var client = CreateApiClientWithToken();

            var response = await client.PostAsJsonAsync(
                "admin/duyurular",
                dto);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var hata = await response.Content.ReadAsStringAsync();

            ModelState.AddModelError(
                "",
                $"Duyuru eklenirken bir hata oluştu. API: {hata}");

            return View(dto);
        }

        // 4. Güncelleme Sayfası
        [HttpGet]
        public async Task<IActionResult> Guncelle(Guid id)
        {
            var client = CreateApiClientWithToken();

            var response = await client.GetAsync(
                $"admin/duyurular/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return NotFound();

            if (!response.IsSuccessStatusCode)
            {
                var hata = await response.Content.ReadAsStringAsync();

                return Content(
                    $"API Hatası: {(int)response.StatusCode} {response.StatusCode}\n\n{hata}");
            }

            var duyuru = await response.Content
                .ReadFromJsonAsync<DuyuruGuncelleDto>();

            if (duyuru == null)
                return NotFound();

            return View(duyuru);
        }

        // 5. Güncelleme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guncelle(
            DuyuruGuncelleDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var client = CreateApiClientWithToken();

            var response = await client.PutAsJsonAsync(
                "admin/duyurular",
                dto);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var hata = await response.Content.ReadAsStringAsync();

            ModelState.AddModelError(
                "",
                $"Güncelleme sırasında bir hata oluştu. API: {hata}");

            return View(dto);
        }

        // 6. Silme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(Guid id)
        {
            var client = CreateApiClientWithToken();

            var response = await client.DeleteAsync(
                $"admin/duyurular/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var hata = await response.Content.ReadAsStringAsync();

                TempData["Hata"] =
                    $"Duyuru silinemedi. API: {hata}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}