using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using ECommerce.Application.DTOs.Menu;
using ECommerce.Application.DTOs.Kategori;
using ECommerce.Application.DTOs.Sayfa;

namespace ECommerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MenuController(IHttpClientFactory httpClientFactory)
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

        // 1. AŞAMA: MENÜ GRUPLARINI LİSTELEME
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync("admin/menuler/gruplar");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var gruplar = JsonSerializer.Deserialize<List<MenuGrupListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(gruplar ?? new List<MenuGrupListeDto>());
            }

            TempData["HataMesaji"] = "Menü grupları yüklenirken bir hata oluştu.";
            return View(new List<MenuGrupListeDto>());
        }

        // YENİ GRUP EKLEME (GET)
        [HttpGet]
        public IActionResult GrupEkle()
        {
            return View(new MenuGrupEkleDto { Konum = "Header" });
        }

        // YENİ GRUP EKLEME (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GrupEkle(MenuGrupEkleDto dto)
        {
            var client = CreateClientWithToken();
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var jsonContent = new StringContent(JsonSerializer.Serialize(dto, jsonOptions), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("admin/menuler/gruplar", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Menü grubu başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }

            TempData["HataMesaji"] = "Menü grubu eklenirken bir hata oluştu.";
            return View(dto);
        }

        // GRUP GÜNCELLEME (GET)
        [HttpGet]
        public async Task<IActionResult> GrupGuncelle(Guid id)
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync($"admin/menuler/gruplar/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var grup = JsonSerializer.Deserialize<MenuGrupGuncelleDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (grup != null) return View(grup);
            }

            TempData["HataMesaji"] = "Menü grubu bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        // GRUP GÜNCELLEME (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GrupGuncelle(MenuGrupGuncelleDto dto)
        {
            var client = CreateClientWithToken();
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var jsonContent = new StringContent(JsonSerializer.Serialize(dto, jsonOptions), Encoding.UTF8, "application/json");

            var response = await client.PutAsync("admin/menuler/gruplar", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Menü grubu güncellendi.";
                return RedirectToAction(nameof(Index));
            }

            TempData["HataMesaji"] = "Güncelleme sırasında bir hata oluştu.";
            return View(dto);
        }

        // GRUP SİLME
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GrupSil(Guid id)
        {
            var client = CreateClientWithToken();
            var response = await client.DeleteAsync($"admin/menuler/gruplar/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Menü grubu başarıyla silindi.";
            }
            else
            {
                TempData["HataMesaji"] = "Sistem menü grupları silinemez! Sadece pasif yapabilirsiniz.";
            }

            return RedirectToAction(nameof(Index));
        }

        // 2. AŞAMA: SEÇİLEN GRUBUN İÇİNE GİRİP ÖĞELERİ (LİNKLERİ) YÖNETME
        [HttpGet]
        public async Task<IActionResult> Detay(Guid grupId)
        {
            var client = CreateClientWithToken();

            // Grup bilgilerini al
            var grupResponse = await client.GetAsync($"admin/menuler/gruplar/{grupId}");
            if (!grupResponse.IsSuccessStatusCode)
            {
                TempData["HataMesaji"] = "Menü grubu bulunamadı.";
                return RedirectToAction(nameof(Index));
            }
            var grupContent = await grupResponse.Content.ReadAsStringAsync();
            var grup = JsonSerializer.Deserialize<MenuGrupGuncelleDto>(grupContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            ViewBag.GrupAdi = grup?.Baslik;
            ViewBag.GrupId = grupId;

            // --- PRO MENÜ SİSTEMİ İÇİN: Kategorileri Çek ---
            var katResponse = await client.GetAsync("kategoriler");
            if (katResponse.IsSuccessStatusCode)
            {
                var katContent = await katResponse.Content.ReadAsStringAsync();
                ViewBag.Kategoriler = JsonSerializer.Deserialize<List<KategoriListeDto>>(katContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
            else
            {
                ViewBag.Kategoriler = new List<KategoriListeDto>();
            }

            // --- PRO MENÜ SİSTEMİ İÇİN: Aktif Sayfaları Çek ---
            var sayfaResponse = await client.GetAsync("sayfalar/aktif-liste");
            if (sayfaResponse.IsSuccessStatusCode)
            {
                var sayfaContent = await sayfaResponse.Content.ReadAsStringAsync();
                ViewBag.Sayfalar = JsonSerializer.Deserialize<List<SayfaListeDto>>(sayfaContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
            else
            {
                ViewBag.Sayfalar = new List<SayfaListeDto>();
            }

            // Gruba ait öğeleri hiyerarşik getir
            var ogelerResponse = await client.GetAsync($"admin/menuler/ogeler/{grupId}");
            var ogeler = new List<MenuItemListeDto>();
            if (ogelerResponse.IsSuccessStatusCode)
            {
                var ogelerContent = await ogelerResponse.Content.ReadAsStringAsync();
                ogeler = JsonSerializer.Deserialize<List<MenuItemListeDto>>(ogelerContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<MenuItemListeDto>();
            }

            return View(ogeler);
        }

        // GRUBA YENİ ÖĞE (LİNK) EKLEME
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OgeEkle(MenuItemEkleDto dto)
        {
            var client = CreateClientWithToken();
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var jsonContent = new StringContent(JsonSerializer.Serialize(dto, jsonOptions), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("admin/menuler/ogeler", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Menü öğesi eklendi.";
            }
            else
            {
                TempData["HataMesaji"] = "Öğe eklenirken hata oluştu.";
            }

            return RedirectToAction(nameof(Detay), new { grupId = dto.MenuGrupId });
        }

        // MENÜ ÖĞESİ GÜNCELLEME (GET)
        [HttpGet]
        public async Task<IActionResult> OgeGuncelle(Guid id, Guid grupId)
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync($"admin/menuler/ogeler/detay/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var oge = JsonSerializer.Deserialize<MenuItemGuncelleDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (oge != null)
                {
                    ViewBag.GrupId = grupId;
                    return View(oge);
                }
            }

            TempData["HataMesaji"] = "Menü öğesi bulunamadı.";
            return RedirectToAction(nameof(Detay), new { grupId = grupId });
        }

        // MENÜ ÖĞESİ GÜNCELLEME (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OgeGuncelle(MenuItemGuncelleDto dto)
        {
            var client = CreateClientWithToken();
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var jsonContent = new StringContent(JsonSerializer.Serialize(dto, jsonOptions), Encoding.UTF8, "application/json");

            var response = await client.PutAsync("admin/menuler/ogeler", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Menü öğesi başarıyla güncellendi.";
            }
            else
            {
                TempData["HataMesaji"] = "Öğe güncellenirken hata oluştu.";
            }

            return RedirectToAction(nameof(Detay), new { grupId = dto.MenuGrupId });
        }

        // MENÜ ÖĞESİNİ SİLME
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OgeSil(Guid id, Guid grupId)
        {
            var client = CreateClientWithToken();
            var response = await client.DeleteAsync($"admin/menuler/ogeler/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Öğe silindi.";
            }
            else
            {
                TempData["HataMesaji"] = "Öğe silinirken hata oluştu.";
            }

            return RedirectToAction(nameof(Detay), new { grupId = grupId });
        }
    }
}