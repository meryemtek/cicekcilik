using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using ECommerce.Application.DTOs.Sepet;
using ECommerce.Application.DTOs.Urun;
using ECommerce.Application.Models;

namespace ECommerce.WebUI.Controllers
{
    public class SepetController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string SepetCookieName = "LidyaSepetId";

        public SepetController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private string GetOrCreateSepetId()
        {
            if (Request.Cookies.TryGetValue(SepetCookieName, out var sepetId) && !string.IsNullOrEmpty(sepetId))
            {
                return sepetId;
            }

            sepetId = "sep_" + Guid.NewGuid().ToString();
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(7),
                HttpOnly = true,
                IsEssential = true
            };
            Response.Cookies.Append(SepetCookieName, sepetId, cookieOptions);
            return sepetId;
        }

        private HttpClient CreateApiClient()
        {
            return _httpClientFactory.CreateClient("ApiClient");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var sepetId = GetOrCreateSepetId();
            var client = CreateApiClient();
            var response = await client.GetAsync($"sepet/{sepetId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var sepet = JsonSerializer.Deserialize<Sepet>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(sepet ?? new Sepet(sepetId));
            }

            return View(new Sepet(sepetId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(SepetEkleDto dto)
        {
            var client = CreateApiClient();
            var referer = Request.Headers["Referer"].ToString();

            if (string.IsNullOrWhiteSpace(dto.Mahalle))
            {
                TempData["HataMesaji"] = "Sipariş vermek için teslimat mahallesi seçmelisiniz.";
                return !string.IsNullOrEmpty(referer) ? Redirect(referer) : RedirectToAction("Index", "Home");
            }

            var urunResponse = await client.GetAsync($"urunler/id-ile/{dto.UrunId}");
            if (!urunResponse.IsSuccessStatusCode)
            {
                TempData["HataMesaji"] = "Ürün bilgileri API'den alınamadı.";
                return !string.IsNullOrEmpty(referer) ? Redirect(referer) : RedirectToAction("Index", "Home");
            }

            var urunContent = await urunResponse.Content.ReadAsStringAsync();
            var urun = JsonSerializer.Deserialize<UrunDetayDto>(urunContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (urun == null)
            {
                TempData["HataMesaji"] = "Ürün detayları çözümlenemedi.";
                return !string.IsNullOrEmpty(referer) ? Redirect(referer) : RedirectToAction("Index", "Home");
            }

            decimal birimFiyat = urun.Fiyat;
            string varyantAdi = "Standart Boy / Model";
            Guid? varyantId = dto.VaryantId;

            if (dto.VaryantId.HasValue && urun.Varyantlar != null && urun.Varyantlar.Any())
            {
                var secilenVaryant = urun.Varyantlar.FirstOrDefault(v => v.Id == dto.VaryantId.Value);
                if (secilenVaryant != null)
                {
                    birimFiyat += secilenVaryant.FiyatFarki;
                    varyantAdi = secilenVaryant.VaryantAdi;
                }
            }

            string gorselUrl = urun.GorselUrl;
            if (!string.IsNullOrEmpty(gorselUrl) && !gorselUrl.StartsWith("http"))
            {
                var baseAddress = client.BaseAddress?.ToString().TrimEnd('/') ?? "https://localhost:7192";
                var uri = new Uri(baseAddress);
                var rootUrl = $"{uri.Scheme}://{uri.Authority}";
                gorselUrl = $"{rootUrl}/{gorselUrl.TrimStart('/')}";
            }

            // ====================================================================
            // KÜLTÜR (LOCALE) HATASINI ENGELLEYEN GÜVENLİ PARSE İŞLEMİ
            // ====================================================================
            decimal teslimatUcreti = 0;
            if (Request.Form.TryGetValue("MahalleFarki", out var mahalleVal) && !string.IsNullOrEmpty(mahalleVal))
            {
                decimal.TryParse(mahalleVal.ToString().Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out teslimatUcreti);
            }
            if (teslimatUcreti < 0) teslimatUcreti = 0;

            decimal vipUcreti = 0;
            if (dto.VipTeslimatAktifMi && Request.Form.TryGetValue("VipFarki", out var vipVal) && !string.IsNullOrEmpty(vipVal))
            {
                decimal.TryParse(vipVal.ToString().Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out vipUcreti);
            }
            if (vipUcreti < 0) vipUcreti = 0;
            // ====================================================================

            var sepetKalemi = new SepetKalemi
            {
                Id = Guid.NewGuid(), // Sepet satırının benzersiz kimliği (Çakışmaları önler)
                UrunId = urun.Id,
                UrunAdi = urun.Ad,
                VaryantId = varyantId,
                VaryantAdi = varyantAdi,
                BirimFiyat = birimFiyat,
                Miktar = dto.Adet < 1 ? 1 : dto.Adet,
                GorselUrl = gorselUrl,
                Il = dto.Il ?? string.Empty,
                Ilce = dto.Ilce ?? string.Empty,
                Mahalle = dto.Mahalle ?? string.Empty,
                TeslimatGunu = dto.TeslimatGunu ?? string.Empty,
                TeslimatSaati = dto.TeslimatSaati ?? string.Empty,
                VipTeslimatAktifMi = dto.VipTeslimatAktifMi,
                TeslimatUcreti = teslimatUcreti,
                VipUcreti = vipUcreti
            };

            var sepetId = GetOrCreateSepetId();
            var jsonContent = new StringContent(JsonSerializer.Serialize(sepetKalemi), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"sepet/{sepetId}/ekle", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Ürün başarıyla sepete eklendi.";
                return RedirectToAction(nameof(Index));
            }

            TempData["HataMesaji"] = "Ürün sepete eklenirken hata oluştu.";
            return !string.IsNullOrEmpty(referer) ? Redirect(referer) : RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Cikar(Guid kalemId) // urunId yerine kalemId oldu
        {
            var sepetId = GetOrCreateSepetId();
            var client = CreateApiClient();
            await client.DeleteAsync($"sepet/{sepetId}/cikar/{kalemId}"); // API'ye kalemId gönderiliyor
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UrunuAyir(Guid id) // View'dan gelen kalemId (id)
        {
            var sepetId = GetOrCreateSepetId();
            var client = CreateApiClient();

            var response = await client.PostAsync($"sepet/{sepetId}/ayir/{id}", null);

            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }

            return BadRequest(new { Hata = "Ürün ayrılırken bir hata oluştu." });
        }

        [HttpPost]
        public async Task<IActionResult> SepetiBirlestir()
        {
            var sepetId = GetOrCreateSepetId();
            var client = CreateApiClient();
            var response = await client.PostAsync($"sepet/{sepetId}/birlestir", null);

            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> KuponUygula(string kuponKodu)
        {
            var sepetId = GetOrCreateSepetId();
            var client = CreateApiClient();

            var jsonContent = new StringContent(JsonSerializer.Serialize(new { kuponKodu }), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"sepet/{sepetId}/kupon-uygula", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "İndirim kuponu başarıyla uygulandı.";
            }
            else
            {
                var errContent = await response.Content.ReadAsStringAsync();
                try
                {
                    using var doc = JsonDocument.Parse(errContent);
                    if (doc.RootElement.TryGetProperty("mesaj", out var mesajProp))
                    {
                        TempData["HataMesaji"] = mesajProp.GetString();
                    }
                    else
                    {
                        TempData["HataMesaji"] = "Geçersiz veya şartları sağlamayan indirim kuponu.";
                    }
                }
                catch
                {
                    TempData["HataMesaji"] = "İndirim kuponu uygulanırken bir hata oluştu.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> KuponKaldir()
        {
            var sepetId = GetOrCreateSepetId();
            var client = CreateApiClient();
            var response = await client.DeleteAsync($"sepet/{sepetId}/kupon-kaldir");

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "İndirim kuponu kaldırıldı.";
            }
            else
            {
                TempData["HataMesaji"] = "Kupon kaldırılırken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
