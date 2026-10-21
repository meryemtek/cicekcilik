using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ECommerce.Application.DTOs.Kategori;
using ECommerce.Application.DTOs.Slider;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SliderController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SliderController(IHttpClientFactory httpClientFactory)
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
        public async Task<IActionResult> Ekle()
        {
            await KategorileriYukleAsync();
            return View(new SliderEkleDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(SliderEkleDto dto, IFormFile? gorselDosyasi)
        {
            await KategorileriYukleAsync(dto.LinkUrl);
            if (gorselDosyasi == null || gorselDosyasi.Length == 0)
            {
                ModelState.AddModelError("gorselDosyasi", "Slider görseli seçin.");
                return View(dto);
            }
            var resimUrl = await GorselKaydetAsync(gorselDosyasi);
            if (resimUrl == null) return View(dto);
            dto.ResimUrl = resimUrl;

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
                if (slider != null)
                {
                    await KategorileriYukleAsync(slider.LinkUrl);
                    return View(slider);
                }
            }

            TempData["HataMesaji"] = "Slider bilgileri bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guncelle(SliderGuncelleDto dto, IFormFile? gorselDosyasi)
        {
            await KategorileriYukleAsync(dto.LinkUrl);
            if (gorselDosyasi != null && gorselDosyasi.Length > 0)
            {
                var resimUrl = await GorselKaydetAsync(gorselDosyasi);
                if (resimUrl == null) return View(dto);
                dto.ResimUrl = resimUrl;
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

        private async Task KategorileriYukleAsync(string? seciliLink = null)
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync("kategoriler");
            var kategoriler = new List<KategoriListeDto>();
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                kategoriler = JsonSerializer.Deserialize<List<KategoriListeDto>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<KategoriListeDto>();
            }

            var secenekler = kategoriler
                .OrderBy(x => x.Ad)
                .Select(x => new SelectListItem
                {
                    Text = x.Ad + (x.AktifMi ? "" : " (pasif)") +
                        (string.IsNullOrWhiteSpace(x.SeoUrl) ? " — SEO adresi yok" : ""),
                    Value = string.IsNullOrWhiteSpace(x.SeoUrl) ? "" : $"/kategori/{x.SeoUrl}",
                    Disabled = string.IsNullOrWhiteSpace(x.SeoUrl)
                })
                .ToList();

            if (!string.IsNullOrWhiteSpace(seciliLink) &&
                !secenekler.Any(x => x.Value == seciliLink))
            {
                secenekler.Insert(0, new SelectListItem
                {
                    Text = $"Mevcut bağlantı: {seciliLink}",
                    Value = seciliLink
                });
            }

            ViewBag.KategoriLinkleri = secenekler;
            ViewBag.KategorilerYuklenemedi = !response.IsSuccessStatusCode;
            ViewBag.KategoriUyarisi = response.IsSuccessStatusCode && kategoriler.Count == 0
                ? "Sistemde kayıtlı kategori bulunamadı."
                : response.IsSuccessStatusCode && secenekler.All(x => x.Disabled)
                    ? "Kategorilerin SEO adresi bulunmuyor. Kategori yönetiminden SEO adreslerini doldurun."
                    : null;
        }

        private async Task<string?> GorselKaydetAsync(IFormFile dosya)
        {
            if (dosya.Length == 0 || dosya.Length > 15 * 1024 * 1024)
            {
                ModelState.AddModelError("gorselDosyasi", "Görsel boyutu en fazla 15 MB olabilir.");
                return null;
            }

            var uzanti = Path.GetExtension(dosya.FileName).ToLowerInvariant();
            var gecerliTurler = new Dictionary<string, string>
            {
                [".jpg"] = "image/jpeg", [".jpeg"] = "image/jpeg",
                [".png"] = "image/png", [".webp"] = "image/webp"
            };
            if (!gecerliTurler.TryGetValue(uzanti, out var icerikTuru) || dosya.ContentType != icerikTuru)
            {
                ModelState.AddModelError("gorselDosyasi", "JPG, PNG veya WEBP görsel yükleyin.");
                return null;
            }

            return await ECommerce.WebUI.Services.ApiGorselYukleyici.YukleAsync(
                _httpClientFactory, User, dosya, "slider");
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
