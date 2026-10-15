using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using ECommerce.Application.DTOs.SiparisGorselOnay;

namespace ECommerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SiparisGorselController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _env;

        public SiparisGorselController(IHttpClientFactory httpClientFactory, IWebHostEnvironment env)
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
        public async Task<IActionResult> Yonet(Guid siparisId)
        {
            ViewBag.SiparisId = siparisId;
            var client = CreateClientWithToken();
            var response = await client.GetAsync($"admin/siparis-gorsel/{siparisId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var gorselDto = JsonSerializer.Deserialize<SiparisGorselOnayDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(gorselDto);
            }

            // Eğer görsel henüz yüklenmemişse boş bir DTO gönderiyoruz
            return View(new SiparisGorselOnayDto { SiparisId = siparisId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GorselYukle(Guid siparisId, IFormFile gorselDosyasi)
        {
            if (gorselDosyasi == null || gorselDosyasi.Length == 0)
            {
                TempData["HataMesaji"] = "Lütfen bir görsel seçiniz.";
                return RedirectToAction(nameof(Yonet), new { siparisId = siparisId });
            }

            // 1. Görseli Sunucuya (WebUI wwwroot) Kaydet
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "siparisler");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var benzersizDosyaAdi = Guid.NewGuid().ToString() + "_" + Path.GetFileName(gorselDosyasi.FileName);
            var dosyaYolu = Path.Combine(uploadsFolder, benzersizDosyaAdi);

            using (var stream = new FileStream(dosyaYolu, FileMode.Create))
            {
                await gorselDosyasi.CopyToAsync(stream);
            }

            var gorselUrl = $"/uploads/siparisler/{benzersizDosyaAdi}";

            // 2. API'ye DTO Gönder
            var dto = new GorselOnayEkleDto
            {
                SiparisId = siparisId,
                GorselUrl = gorselUrl
            };

            var client = CreateClientWithToken();
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var jsonContent = new StringContent(JsonSerializer.Serialize(dto, jsonOptions), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("admin/siparis-gorsel", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Sipariş görseli başarıyla yüklendi ve müşteriye onaya hazır hale geldi.";
            }
            else
            {
                TempData["HataMesaji"] = "Görsel sisteme yüklendi ancak API'ye kaydedilirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Yonet), new { siparisId = siparisId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(Guid siparisId)
        {
            var client = CreateClientWithToken();
            var response = await client.DeleteAsync($"admin/siparis-gorsel/{siparisId}");

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Sipariş görseli başarıyla silindi.";
            }
            else
            {
                TempData["HataMesaji"] = "Görsel silinirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Yonet), new { siparisId = siparisId });
        }
    }
}