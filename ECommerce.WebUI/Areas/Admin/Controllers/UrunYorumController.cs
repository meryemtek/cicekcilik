using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using ECommerce.Application.DTOs.UrunYorum; // Projedeki namespace'e göre düzenlenebilir

namespace ECommerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UrunYorumController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UrunYorumController(IHttpClientFactory httpClientFactory)
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
            var response = await client.GetAsync("admin/urun-yorumlari");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var yorumlar = JsonSerializer.Deserialize<List<UrunYorumListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(yorumlar ?? new List<UrunYorumListeDto>());
            }

            TempData["HataMesaji"] = "Yorum listesi yüklenirken bir hata oluştu.";
            return View(new List<UrunYorumListeDto>());
        }

        [HttpGet]
        public async Task<IActionResult> Guncelle(Guid id)
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync($"admin/urun-yorumlari/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var yorum = JsonSerializer.Deserialize<UrunYorumGuncelleDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (yorum != null) return View(yorum);
            }

            TempData["HataMesaji"] = "Yorum detayları bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guncelle(UrunYorumGuncelleDto dto)
        {
            var client = CreateClientWithToken();
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var jsonContent = new StringContent(JsonSerializer.Serialize(dto, jsonOptions), Encoding.UTF8, "application/json");

            var response = await client.PutAsync("admin/urun-yorumlari", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Yorum durumu ve içeriği başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }

            TempData["HataMesaji"] = "Yorum güncellenirken bir hata oluştu.";
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(Guid id)
        {
            var client = CreateClientWithToken();
            var response = await client.DeleteAsync($"admin/urun-yorumlari/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Yorum başarıyla silindi.";
            }
            else
            {
                TempData["HataMesaji"] = "Yorum silinirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}