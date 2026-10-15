using Microsoft.AspNetCore.Authorization;
using ECommerce.Application.DTOs.Kupon; // Kendi namespace'ine göre ayarlandığını varsayıyorum
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;

namespace ECommerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class KuponController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public KuponController(IHttpClientFactory httpClientFactory)
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
            var response = await client.GetAsync("admin/kuponlar");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var kuponlar = JsonSerializer.Deserialize<List<KuponListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(kuponlar ?? new List<KuponListeDto>());
            }

            return View(new List<KuponListeDto>());
        }

        [HttpGet]
        public IActionResult Ekle()
        {
            return View(new KuponEkleDto { BitisTarihi = DateTime.Now.AddDays(7), AktifMi = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(KuponEkleDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var client = CreateClientWithToken();
            var jsonContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("admin/kuponlar", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "İndirim kuponu başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }

            TempData["HataMesaji"] = "Kupon eklenirken bir hata oluştu.";
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Guncelle(Guid id)
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync($"admin/kuponlar/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var kupon = JsonSerializer.Deserialize<KuponGuncelleDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (kupon != null)
                {
                    return View(kupon);
                }
            }

            TempData["HataMesaji"] = "Güncellenmek istenen kupon bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guncelle(KuponGuncelleDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var client = CreateClientWithToken();
            var jsonContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

            var response = await client.PutAsync("admin/kuponlar", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "İndirim kuponu başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }

            TempData["HataMesaji"] = "Kupon güncellenirken bir hata oluştu.";
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(Guid id)
        {
            var client = CreateClientWithToken();
            var response = await client.DeleteAsync($"admin/kuponlar/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "İndirim kuponu başarıyla silindi.";
            }
            else
            {
                TempData["HataMesaji"] = "Kupon silinirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}