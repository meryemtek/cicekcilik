using ECommerce.Application.DTOs.Kategori;
using ECommerce.Application.DTOs.OneCikan;
using ECommerce.Application.DTOs.Urun;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace ECommerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OneCikanAdminController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OneCikanAdminController(IHttpClientFactory httpClientFactory)
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

            // 1. Ayarları çek
            var ayarResponse = await client.GetAsync("onecikanlar/ayarlar");
            var model = new OneCikanDto { TumUrunleriGoster = true };
            if (ayarResponse.IsSuccessStatusCode)
            {
                var content = await ayarResponse.Content.ReadAsStringAsync();
                model = JsonSerializer.Deserialize<OneCikanDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? model;
            }

            // 2. Kategorileri çek
            var katResponse = await client.GetAsync("kategoriler");
            var kategoriler = new List<KategoriListeDto>();
            if (katResponse.IsSuccessStatusCode)
            {
                var katContent = await katResponse.Content.ReadAsStringAsync();
                kategoriler = JsonSerializer.Deserialize<List<KategoriListeDto>>(katContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<KategoriListeDto>();
            }

            // 3. Ürünleri çek (Ürün bazlı arama ve seçim için)
            var urunResponse = await client.GetAsync("urunler");
            var urunler = new List<UrunListeDto>();
            if (urunResponse.IsSuccessStatusCode)
            {
                var urunContent = await urunResponse.Content.ReadAsStringAsync();
                urunler = JsonSerializer.Deserialize<List<UrunListeDto>>(urunContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<UrunListeDto>();
            }

            ViewBag.Kategoriler = kategoriler.Where(x => x.AktifMi).ToList();
            ViewBag.Urunler = urunler.Where(x => x.AktifMi).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(OneCikanDto model, List<Guid>? secilenKategoriIdleri, List<Guid>? secilenUrunIdleri)
        {
            model.SecilenKategoriIdleri = secilenKategoriIdleri ?? new List<Guid>();
            model.SecilenUrunIdleri = secilenUrunIdleri ?? new List<Guid>();

            var client = CreateClientWithToken();
            var response = await client.PostAsJsonAsync("onecikanlar/ayarlar", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Öne çıkan ürün ayarları başarıyla kaydedildi.";
            }
            else
            {
                var hataDetayi = await response.Content.ReadAsStringAsync();
                TempData["HataMesaji"] = $"Ayarlar kaydedilirken hata oluştu: {hataDetayi}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}