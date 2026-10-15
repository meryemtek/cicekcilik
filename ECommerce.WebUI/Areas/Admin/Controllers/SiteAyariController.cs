using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using System.Globalization;
using ECommerce.Application.DTOs.SiteAyari;

namespace ECommerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SiteAyariController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _env;

        public SiteAyariController(IHttpClientFactory httpClientFactory, IWebHostEnvironment env)
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
            var response = await client.GetAsync("admin/site-ayarlari");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var ayarlar = JsonSerializer.Deserialize<SiteAyariDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(ayarlar ?? new SiteAyariDto());
            }

            TempData["HataMesaji"] = "Site ayarları yüklenirken bir hata oluştu.";
            return View(new SiteAyariDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guncelle(SiteAyariDto dto, IFormFile? logoDosyasi)
        {
            // KUSURSUZ VE HATASIZ DECIMAL PARSE İŞLEMİ (100x Katlanma Sorununu Çözer)
            if (Request.Form.TryGetValue("VipTeslimatUcreti", out var vipVal) && !string.IsNullOrEmpty(vipVal))
            {
                string valStr = vipVal.ToString().Trim();

                // Türkçe format (250,00 veya 1.000,00) veya İngilizce format (1000.00) desteği
                var trCulture = CultureInfo.GetCultureInfo("tr-TR");
                var invCulture = CultureInfo.InvariantCulture;

                if (decimal.TryParse(valStr, NumberStyles.Any, trCulture, out var parsedTr))
                {
                    dto.VipTeslimatUcreti = parsedTr;
                }
                else if (decimal.TryParse(valStr.Replace(".", "").Replace(',', '.'), NumberStyles.Any, invCulture, out var parsedInv))
                {
                    dto.VipTeslimatUcreti = parsedInv;
                }
            }

            // Eğer yeni bir logo dosyası yüklendiyse wwwroot altına kaydediyoruz
            if (logoDosyasi != null && logoDosyasi.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "logo");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var benzersizAd = Guid.NewGuid().ToString() + "_" + Path.GetFileName(logoDosyasi.FileName);
                var dosyaYolu = Path.Combine(uploadsFolder, benzersizAd);

                using (var stream = new FileStream(dosyaYolu, FileMode.Create))
                {
                    await logoDosyasi.CopyToAsync(stream);
                }

                dto.LogoUrl = $"/uploads/logo/{benzersizAd}";
            }

            var client = CreateClientWithToken();
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var jsonContent = new StringContent(JsonSerializer.Serialize(dto, jsonOptions), Encoding.UTF8, "application/json");

            var response = await client.PutAsync("admin/site-ayarlari", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Site ayarları ve entegrasyonlar başarıyla kaydedildi.";
            }
            else
            {
                TempData["HataMesaji"] = "Ayarlar kaydedilirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}