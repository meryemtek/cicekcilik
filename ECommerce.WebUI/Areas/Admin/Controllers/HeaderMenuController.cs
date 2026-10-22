using ECommerce.Application.DTOs.Kategori;
using ECommerce.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ECommerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HeaderMenuController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Sabit 6 Ana Menü Tanımı (Sırası, Başlığı, SeoUrl ve İkonu)
        private static readonly List<(string SeoUrl, string Baslik, string IconClass)> SabitAnaMenuler = new()
        {
            ("gonderim-amaci", "GÖNDERİM AMACI", "bi-heart"),
            ("cicekler", "ÇİÇEKLER", "bi-flower2"),
            ("dogum-gunu", "DOĞUM GÜNÜ", "bi-balloon"),
            ("saksi-cicekleri", "SAKSI ÇİÇEKLERİ", "bi-flower1"),
            ("haftanin-cicekleri", "HAFTANIN ÇİÇEKLERİ", "bi-bookmark-star"),
            ("kutuda-cicekler", "KUTUDA ÇİÇEKLER", "bi-box-seam")
        };

        public HeaderMenuController(IHttpClientFactory httpClientFactory)
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
            var kategoriler = new List<KategoriListeDto>();
            try
            {
                var response = await client.GetAsync("kategoriler");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    kategoriler = JsonSerializer.Deserialize<List<KategoriListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                }
                else
                {
                    TempData["HataMesaji"] = "Alt kategoriler API'den yüklenemedi. Boş görünmesi kayıtların silindiği anlamına gelmez; lütfen sayfayı yenileyin.";
                }
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                TempData["HataMesaji"] = "Alt kategoriler geçici bağlantı sorunu nedeniyle yüklenemedi. Boş görünmesi kayıtların silindiği anlamına gelmez; lütfen sayfayı yenileyin.";
            }

            var model = new HeaderMenuYonetimViewModel();

            foreach (var tanim in SabitAnaMenuler)
            {
                // Modeldeki ana kategoriyi bul
                var anaKategori = kategoriler.FirstOrDefault(x =>
                    x.UstKategoriId == null && (
                        x.SeoUrl == tanim.SeoUrl ||
                        (tanim.SeoUrl == "cicekler" && x.SeoUrl == "cicek") ||
                        (tanim.SeoUrl == "saksi-cicekleri" && x.SeoUrl == "saksida-cicekler") ||
                        x.Ad.Replace(" ", "").Equals(tanim.Baslik.Replace(" ", ""), StringComparison.OrdinalIgnoreCase)
                    )
                );

                var grup = new HeaderMenuGrupViewModel
                {
                    Id = anaKategori?.Id ?? Guid.Empty,
                    Baslik = tanim.Baslik,
                    SeoUrl = tanim.SeoUrl,
                    IconClass = tanim.IconClass,
                    AltMenuler = anaKategori != null
                        ? kategoriler.Where(x => x.UstKategoriId == anaKategori.Id).OrderBy(x => x.Ad).ToList()
                        : new List<KategoriListeDto>()
                };

                model.AnaMenuler.Add(grup);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AltMenuEkle(string anaMenuSeoUrl, string altMenuAdi)
        {
            var anaMenu = SabitAnaMenuler.FirstOrDefault(x => x.SeoUrl == anaMenuSeoUrl);
            if (string.IsNullOrEmpty(anaMenu.SeoUrl) || string.IsNullOrWhiteSpace(altMenuAdi))
            {
                TempData["HataMesaji"] = "Lütfen ana menüyü seçin ve alt menü başlığını girin.";
                return RedirectToAction(nameof(Index));
            }

            var client = CreateClientWithToken();
            var kategoriResponse = await client.GetAsync("kategoriler");
            if (!kategoriResponse.IsSuccessStatusCode)
            {
                TempData["HataMesaji"] = "Kategoriler alınamadı. Lütfen tekrar deneyin.";
                return RedirectToAction(nameof(Index));
            }

            var kategoriIcerigi = await kategoriResponse.Content.ReadAsStringAsync();
            var kategoriler = JsonSerializer.Deserialize<List<KategoriListeDto>>(
                kategoriIcerigi,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            var anaKategori = kategoriler.FirstOrDefault(x =>
                x.UstKategoriId == null && (
                    x.SeoUrl == anaMenu.SeoUrl ||
                    (anaMenu.SeoUrl == "cicekler" && x.SeoUrl == "cicek") ||
                    (anaMenu.SeoUrl == "saksi-cicekleri" && x.SeoUrl == "saksida-cicekler") ||
                    x.Ad.Replace(" ", "").Equals(anaMenu.Baslik.Replace(" ", ""), StringComparison.OrdinalIgnoreCase)));

            if (anaKategori == null)
            {
                var anaKategoriDto = new KategoriEkleDto
                {
                    Ad = anaMenu.Baslik,
                    SeoUrl = anaMenu.SeoUrl,
                    Aciklama = string.Empty,
                    GorselUrl = string.Empty,
                    AktifMi = true
                };

                var anaKategoriJson = new StringContent(
                    JsonSerializer.Serialize(anaKategoriDto),
                    Encoding.UTF8,
                    "application/json");
                var anaKategoriResponse = await client.PostAsync("kategoriler", anaKategoriJson);

                if (!anaKategoriResponse.IsSuccessStatusCode)
                {
                    TempData["HataMesaji"] = "Ana menü kategorisi oluşturulamadı. Lütfen Kategoriler bölümünü kontrol edin.";
                    return RedirectToAction(nameof(Index));
                }

                var yeniAnaKategori = await anaKategoriResponse.Content.ReadFromJsonAsync<JsonElement>();
                if (!yeniAnaKategori.TryGetProperty("id", out var anaKategoriIdElement) ||
                    !anaKategoriIdElement.TryGetGuid(out var anaKategoriId))
                {
                    TempData["HataMesaji"] = "Ana menü oluşturuldu ancak kimliği alınamadı. Sayfayı yenileyip tekrar deneyin.";
                    return RedirectToAction(nameof(Index));
                }

                anaKategori = new KategoriListeDto
                {
                    Id = anaKategoriId,
                    Ad = anaMenu.Baslik,
                    SeoUrl = anaMenu.SeoUrl,
                    AktifMi = true
                };
            }

            var seoUrl = ToSeoUrl(altMenuAdi.Trim());

            var dto = new KategoriEkleDto
            {
                Ad = altMenuAdi.Trim(),
                SeoUrl = seoUrl,
                Aciklama = string.Empty,
                GorselUrl = string.Empty,
                AktifMi = true,
                UstKategoriId = anaKategori.Id
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("kategoriler", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = $"'{altMenuAdi}' alt menüsü başarıyla eklendi.";
            }
            else
            {
                TempData["HataMesaji"] = "Alt menü eklenirken bir hata oluştu. Aynı isimde bir kategori mevcut olabilir.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AltMenuGuncelle(Guid id, string ad, bool aktifMi)
        {
            if (id == Guid.Empty || string.IsNullOrWhiteSpace(ad))
            {
                TempData["HataMesaji"] = "Geçersiz alt menü bilgisi.";
                return RedirectToAction(nameof(Index));
            }

            var client = CreateClientWithToken();
            var getRes = await client.GetAsync($"kategoriler/{id}");
            if (!getRes.IsSuccessStatusCode)
            {
                TempData["HataMesaji"] = "Güncellenecek alt menü bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            var content = await getRes.Content.ReadAsStringAsync();
            var mevcut = JsonSerializer.Deserialize<KategoriGuncelleDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (mevcut == null)
            {
                TempData["HataMesaji"] = "Kategori verisi çözümlenemedi.";
                return RedirectToAction(nameof(Index));
            }

            mevcut.Ad = ad.Trim();
            mevcut.AktifMi = aktifMi;

            var jsonContent = new StringContent(JsonSerializer.Serialize(mevcut), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("kategoriler", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Alt menü başarıyla güncellendi.";
            }
            else
            {
                TempData["HataMesaji"] = "Alt menü güncellenirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AltMenuSil(Guid id)
        {
            if (id == Guid.Empty)
            {
                TempData["HataMesaji"] = "Geçersiz alt menü ID'si.";
                return RedirectToAction(nameof(Index));
            }

            var client = CreateClientWithToken();
            var response = await client.DeleteAsync($"kategoriler/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Alt menü başarıyla silindi.";
            }
            else
            {
                TempData["HataMesaji"] = "Alt menü silinemedi. Bu alt kategoriye bağlı ürünler bulunuyor olabilir.";
            }

            return RedirectToAction(nameof(Index));
        }

        private static string ToSeoUrl(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            text = text.ToLowerInvariant()
                .Replace("ı", "i")
                .Replace("ğ", "g")
                .Replace("ü", "u")
                .Replace("ş", "s")
                .Replace("ö", "o")
                .Replace("ç", "c");

            text = Regex.Replace(text, @"[^a-z0-9\s-]", "");
            text = Regex.Replace(text, @"\s+", "-").Trim('-');

            return text;
        }
    }
}
