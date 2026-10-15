using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using ECommerce.Application.DTOs.Siparis;

namespace ECommerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SiparisController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SiparisController(IHttpClientFactory httpClientFactory)
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
            var response = await client.GetAsync("admin/siparisler");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var siparisler = JsonSerializer.Deserialize<List<SiparisListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(siparisler ?? new List<SiparisListeDto>());
            }

            TempData["HataMesaji"] = "Sipariş listesi çekilirken bir hata oluştu.";
            return View(new List<SiparisListeDto>());
        }

        [HttpGet]
        public async Task<IActionResult> Detay(Guid id)
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync($"admin/siparisler/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var detay = JsonSerializer.Deserialize<SiparisDetayDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (detay != null)
                {
                    return View(detay);
                }
            }

            TempData["HataMesaji"] = "Sipariş detayları bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DurumGuncelle(SiparisDurumGuncelleDto dto)
        {
            var client = CreateClientWithToken();
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var jsonContent = new StringContent(JsonSerializer.Serialize(dto, jsonOptions), Encoding.UTF8, "application/json");

            var response = await client.PutAsync("admin/siparisler/durum-guncelle", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Sipariş durumu başarıyla güncellendi.";
            }
            else
            {
                TempData["HataMesaji"] = "Sipariş durumu güncellenirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Detay), new { id = dto.Id });
        }
    }
}