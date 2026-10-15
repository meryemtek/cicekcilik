using ECommerce.Application.DTOs.KategoriBanner;
using ECommerce.Application.DTOs.Kategori;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace ECommerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class KategoriBannerAdminController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public KategoriBannerAdminController(IHttpClientFactory httpClientFactory)
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

        private string GetFullImageUrl(string gorselUrl)
        {
            if (string.IsNullOrWhiteSpace(gorselUrl)) return string.Empty;
            if (gorselUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return gorselUrl;
            var client = _httpClientFactory.CreateClient("ApiClient");
            var baseUrl = client.BaseAddress?.GetLeftPart(UriPartial.Authority) ?? "https://localhost:7001";
            return $"{baseUrl.TrimEnd('/')}/{gorselUrl.TrimStart('/')}";
        }

        private async Task<string> ApiyeGorselYukleAsync(IFormFile dosya, string altKlasor)
        {
            if (dosya == null || dosya.Length == 0) return string.Empty;
            var client = CreateClientWithToken();
            using var content = new MultipartFormDataContent();
            var fileStreamContent = new StreamContent(dosya.OpenReadStream());
            fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(dosya.ContentType);
            content.Add(fileStreamContent, "dosya", dosya.FileName);
            content.Add(new StringContent(altKlasor), "altKlasor");
            var response = await client.PostAsync("admin/dosyalar/gorsel-yukle", content);
            if (response.IsSuccessStatusCode)
            {
                var respContent = await response.Content.ReadAsStringAsync();
                try
                {
                    using var doc = JsonDocument.Parse(respContent);
                    foreach (var prop in doc.RootElement.EnumerateObject())
                    {
                        if (prop.Name.Equals("Url", StringComparison.OrdinalIgnoreCase) ||
                            prop.Name.Equals("Path", StringComparison.OrdinalIgnoreCase) ||
                            prop.Name.Equals("FilePath", StringComparison.OrdinalIgnoreCase))
                        {
                            return prop.Value.GetString() ?? string.Empty;
                        }
                    }
                }
                catch
                {
                    if (!string.IsNullOrWhiteSpace(respContent) && (respContent.StartsWith("http") || respContent.StartsWith("/")))
                    {
                        return respContent.Trim('"');
                    }
                }
            }
            return string.Empty;
        }

        private async Task KategorileriViewBageYukleAsync()
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync("kategoriler");
            var kategoriler = new List<KategoriListeDto>();
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                kategoriler = JsonSerializer.Deserialize<List<KategoriListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<KategoriListeDto>();
            }
            ViewBag.Kategoriler = new SelectList(kategoriler.Where(x => x.AktifMi), "Id", "Ad");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync("kategori-bannerlar");
            var liste = new List<KategoriBannerDto>();

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                liste = JsonSerializer.Deserialize<List<KategoriBannerDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<KategoriBannerDto>();

                // KRİTİK DÜZELTME: Görsel yollarını tam URL'e dönüştürüyoruz ki admin listesinde resimler görünsün!
                foreach (var item in liste)
                {
                    if (!string.IsNullOrWhiteSpace(item.GorselUrl))
                    {
                        item.GorselUrl = GetFullImageUrl(item.GorselUrl);
                    }
                }
            }
            return View(liste);
        }

        [HttpGet]
        public async Task<IActionResult> Ekle()
        {
            await KategorileriViewBageYukleAsync();
            return View(new KategoriBannerEkleGuncelleDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(KategoriBannerEkleGuncelleDto model, IFormFile? gorselDosyasi)
        {
            if (gorselDosyasi != null)
            {
                model.GorselUrl = await ApiyeGorselYukleAsync(gorselDosyasi, "kategori-banner");
            }

            var client = CreateClientWithToken();
            var response = await client.PostAsJsonAsync("kategori-bannerlar", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Kategori Banner başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Eklenirken bir hata oluştu.");
            await KategorileriViewBageYukleAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Guncelle(Guid id)
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync($"kategori-bannerlar/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var model = JsonSerializer.Deserialize<KategoriBannerEkleGuncelleDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (model != null)
                {
                    model.GorselUrl = GetFullImageUrl(model.GorselUrl);
                    await KategorileriViewBageYukleAsync();
                    return View(model);
                }
            }

            TempData["HataMesaji"] = "Banner bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guncelle(KategoriBannerEkleGuncelleDto model, IFormFile? gorselDosyasi)
        {
            ModelState.Remove("GorselUrl");

            if (gorselDosyasi != null && gorselDosyasi.Length > 0)
            {
                model.GorselUrl = await ApiyeGorselYukleAsync(gorselDosyasi, "kategori-banner");
            }
            else
            {
                var clientCheck = CreateClientWithToken();
                var resCheck = await clientCheck.GetAsync($"kategori-bannerlar/{model.Id}");
                if (resCheck.IsSuccessStatusCode)
                {
                    var contentCheck = await resCheck.Content.ReadAsStringAsync();
                    var eski = JsonSerializer.Deserialize<KategoriBannerEkleGuncelleDto>(contentCheck, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (eski != null) model.GorselUrl = eski.GorselUrl;
                }
            }

            var client = CreateClientWithToken();
            var response = await client.PutAsJsonAsync("kategori-bannerlar", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Banner başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Güncellenirken hata oluştu.");
            await KategorileriViewBageYukleAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Sil(Guid id)
        {
            var client = CreateClientWithToken();
            var response = await client.DeleteAsync($"kategori-bannerlar/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Banner başarıyla silindi.";
            }
            else
            {
                TempData["HataMesaji"] = "Silinirken hata oluştu.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}