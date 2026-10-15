using Microsoft.AspNetCore.Authorization;
using ECommerce.Application.DTOs.SemtTeslimatBolgesi;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace ECommerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TeslimatController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TeslimatController(IHttpClientFactory httpClientFactory)
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
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DataTablesVeriGetir()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault() ?? "";

            int pageSize = string.IsNullOrEmpty(length) ? 10 : Convert.ToInt32(length);
            int skip = string.IsNullOrEmpty(start) ? 0 : Convert.ToInt32(start);

            // DataTables'ta "Tümü" seçilirse length -1 gelir. Hata vermemesi için düzeltiyoruz.
            if (pageSize < 0) pageSize = 10000;

            var client = CreateClientWithToken();

            var url = $"admin/semt-teslimat/sayfali?start={skip}&length={pageSize}&aramaDegeri={Uri.EscapeDataString(searchValue)}";
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var hata = await response.Content.ReadAsStringAsync();
                return Json(new { draw = draw, recordsTotal = 0, recordsFiltered = 0, data = new List<object>(), error = $"API Hatası: {response.StatusCode} - {hata}" });
            }

            var content = await response.Content.ReadAsStringAsync();
            var sonuc = JsonSerializer.Deserialize<SemtBolgeSayfaliSonucDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (sonuc == null || sonuc.Data == null)
            {
                return Json(new { draw = draw, recordsTotal = 0, recordsFiltered = 0, data = new List<object>() });
            }

            // KRİTİK NOKTA: DataTables'ın beklediği isimlendirmeyi (camelCase) manuel ve kesin olarak ayarlıyoruz.
            var formatliVeriler = sonuc.Data.Select(x => new
            {
                id = x.Id,
                ilAdi = x.IlAdi ?? "",
                ilceAdi = x.IlceAdi ?? "",
                mahalleAdi = x.MahalleAdi ?? "",
                ekTeslimatUcreti = x.EkTeslimatUcreti,
                aktifMi = x.AktifMi
            }).ToList();

            return Json(new
            {
                draw = draw,
                recordsTotal = sonuc.RecordsTotal,
                recordsFiltered = sonuc.RecordsFiltered,
                data = formatliVeriler
            });
        }

        [HttpGet]
        public IActionResult Ekle()
        {
            return View(new SemtBolgeEkleDto { AktifMi = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(SemtBolgeEkleDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var client = CreateClientWithToken();

            // DİKKAT: Web API'nin anlayabilmesi için veriyi camelCase olarak dönüştürüp gönderiyoruz.
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var jsonContent = new StringContent(JsonSerializer.Serialize(dto, jsonOptions), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("admin/semt-teslimat", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Teslimat bölgesi başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }

            TempData["HataMesaji"] = "Bölge eklenirken bir hata oluştu.";
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Guncelle(Guid id)
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync($"admin/semt-teslimat/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var bolge = JsonSerializer.Deserialize<SemtBolgeGuncelleDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (bolge != null)
                {
                    return View(bolge);
                }
            }

            TempData["HataMesaji"] = "Güncellenmek istenen bölge bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guncelle(SemtBolgeGuncelleDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var client = CreateClientWithToken();

            // Aynı korumayı buraya da ekliyoruz
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var jsonContent = new StringContent(JsonSerializer.Serialize(dto, jsonOptions), Encoding.UTF8, "application/json");

            var response = await client.PutAsync("admin/semt-teslimat", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Teslimat bölgesi başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }

            TempData["HataMesaji"] = "Bölge güncellenirken bir hata oluştu.";
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(Guid id)
        {
            var client = CreateClientWithToken();
            var response = await client.DeleteAsync($"admin/semt-teslimat/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Teslimat bölgesi başarıyla silindi.";
            }
            else
            {
                TempData["HataMesaji"] = "Bölge silinirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }

        // --- YENİ: AJAX İÇİN ADRES HAVUZU KÖPRÜ ENDPOINTLERİ ---

        [HttpGet]
        public async Task<IActionResult> IlleriGetir()
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync("admin/adres-havuzu/iller");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            return Json(new List<string>());
        }

        [HttpGet]
        public async Task<IActionResult> IlceleriGetir(string ilAdi)
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync($"admin/adres-havuzu/ilceler?ilAdi={Uri.EscapeDataString(ilAdi ?? "")}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            return Json(new List<string>());
        }

        [HttpGet]
        public async Task<IActionResult> MahalleleriGetir(string ilAdi, string ilceAdi)
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync($"admin/adres-havuzu/mahalleler?ilAdi={Uri.EscapeDataString(ilAdi ?? "")}&ilceAdi={Uri.EscapeDataString(ilceAdi ?? "")}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            return Json(new List<string>());
        }
    }
}