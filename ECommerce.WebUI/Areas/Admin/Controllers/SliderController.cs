using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ECommerce.Application.DTOs.Slider;

namespace ECommerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SliderController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _env;

        public SliderController(IHttpClientFactory httpClientFactory, IWebHostEnvironment env)
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
            var response = await client.GetAsync("admin/slider");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var sliderlar = JsonSerializer.Deserialize<List<SliderListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(sliderlar ?? new List<SliderListeDto>());
            }

            TempData["HataMesaji"] = "Slider listesi yüklenirken bir hata oluştu.";
            return View(new List<SliderListeDto>());
        }

        [HttpGet]
        public IActionResult Ekle()
        {
            return View(new SliderEkleDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(SliderEkleDto dto, IFormFile? gorselDosyasi)
        {
            if (gorselDosyasi != null && gorselDosyasi.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "slider");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var benzersizAd = Guid.NewGuid().ToString() + "_" + Path.GetFileName(gorselDosyasi.FileName);
                var dosyaYolu = Path.Combine(uploadsFolder, benzersizAd);

                using (var stream = new FileStream(dosyaYolu, FileMode.Create))
                {
                    await gorselDosyasi.CopyToAsync(stream);
                }

                dto.ResimUrl = $"/uploads/slider/{benzersizAd}"; // GorselUrl yerine ResimUrl yapıldı
            }

            var client = CreateClientWithToken();
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var jsonContent = new StringContent(JsonSerializer.Serialize(dto, jsonOptions), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("admin/slider", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Slider başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }

            TempData["HataMesaji"] = "Slider eklenirken bir hata oluştu.";
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Guncelle(Guid id)
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync($"admin/slider/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var slider = JsonSerializer.Deserialize<SliderGuncelleDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (slider != null) return View(slider);
            }

            TempData["HataMesaji"] = "Slider bilgileri bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guncelle(SliderGuncelleDto dto, IFormFile? gorselDosyasi)
        {
            if (gorselDosyasi != null && gorselDosyasi.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "slider");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var benzersizAd = Guid.NewGuid().ToString() + "_" + Path.GetFileName(gorselDosyasi.FileName);
                var dosyaYolu = Path.Combine(uploadsFolder, benzersizAd);

                using (var stream = new FileStream(dosyaYolu, FileMode.Create))
                {
                    await gorselDosyasi.CopyToAsync(stream);
                }

                dto.ResimUrl = $"/uploads/slider/{benzersizAd}"; // GorselUrl yerine ResimUrl yapıldı
            }
            // Eğer resim seçilmemişse, dto içindeki mevcut ResimUrl (View'dan hidden olarak gelen) aynen kalır.

            var client = CreateClientWithToken();
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var jsonContent = new StringContent(JsonSerializer.Serialize(dto, jsonOptions), Encoding.UTF8, "application/json");

            var response = await client.PutAsync("admin/slider", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Slider başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }

            TempData["HataMesaji"] = "Slider güncellenirken bir hata oluştu.";
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(Guid id)
        {
            var client = CreateClientWithToken();
            var response = await client.DeleteAsync($"admin/slider/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Slider başarıyla silindi.";
            }
            else
            {
                TempData["HataMesaji"] = "Slider silinirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}