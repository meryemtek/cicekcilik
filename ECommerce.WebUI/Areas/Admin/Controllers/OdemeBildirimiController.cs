using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using ECommerce.Application.DTOs.OdemeBildirimi;

namespace ECommerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OdemeBildirimiController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OdemeBildirimiController(IHttpClientFactory httpClientFactory)
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
            var response = await client.GetAsync("admin/odeme-bildirimleri");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var list = JsonSerializer.Deserialize<List<OdemeBildirimiListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(list);
            }

            return View(new List<OdemeBildirimiListeDto>());
        }

        [HttpGet]
        public async Task<IActionResult> Detay(Guid id)
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync($"admin/odeme-bildirimleri/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var detay = JsonSerializer.Deserialize<OdemeBildirimiDetayDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (detay != null) return View(detay);
            }

            TempData["HataMesaji"] = "Bildirim bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Sil(Guid id)
        {
            var client = CreateClientWithToken();
            var response = await client.DeleteAsync($"admin/odeme-bildirimleri/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Bildirim başarıyla silindi.";
            }
            else
            {
                TempData["HataMesaji"] = "Silme sırasında hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> DekontIndir(Guid id)
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync($"admin/odeme-bildirimleri/{id}/dekont");

            if (response.IsSuccessStatusCode)
            {
                var fileStream = await response.Content.ReadAsStreamAsync();
                var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
                var fileName = response.Content.Headers.ContentDisposition?.FileName ?? $"dekont-{id}.png";

                return File(fileStream, contentType, fileName);
            }

            TempData["HataMesaji"] = "Dekont dosyası bulunamadı veya indirilemedi.";
            return RedirectToAction(nameof(Detay), new { id });
        }
    }
}