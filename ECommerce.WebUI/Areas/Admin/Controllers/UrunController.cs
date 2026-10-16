using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Text.Json;
using ECommerce.Application.DTOs.Urun;
using ECommerce.Application.DTOs.Kategori;

namespace ECommerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UrunController : Controller
    {
        private static readonly HashSet<string> AnaMenuSeoUrlListesi = new(StringComparer.OrdinalIgnoreCase)
        {
            "gonderim-amaci",
            "cicekler",
            "dogum-gunu",
            "saksi-cicekleri",
            "haftanin-cicekleri",
            "kutuda-cicekler"
        };

        private readonly IHttpClientFactory _httpClientFactory;
        public UrunController(IHttpClientFactory httpClientFactory)
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
                var responseContent = await response.Content.ReadAsStringAsync();

                try
                {
                    using var doc = JsonDocument.Parse(responseContent);
                    var root = doc.RootElement;

                    // API'den gelebilecek olası farklı property isimlerini (Url, url, filePath vb.) büyük/küçük harf duyarsız kontrol ediyoruz
                    foreach (var property in root.EnumerateObject())
                    {
                        if (property.Name.Equals("Url", StringComparison.OrdinalIgnoreCase) ||
                            property.Name.Equals("Path", StringComparison.OrdinalIgnoreCase) ||
                            property.Name.Equals("FilePath", StringComparison.OrdinalIgnoreCase))
                        {
                            return property.Value.GetString() ?? string.Empty;
                        }
                    }
                }
                catch
                {
                    // Eğer dönen içerik direkt düz bir string URL ise (JSON objesi değilse) doğrudan geri döndür
                    if (!string.IsNullOrWhiteSpace(responseContent) && (responseContent.StartsWith("http") || responseContent.StartsWith("/")))
                    {
                        return responseContent.Trim('"');
                    }
                }
            }

            return string.Empty;
        }

        private string GetFullImageUrl(string gorselUrl)
        {
            if (string.IsNullOrWhiteSpace(gorselUrl)) return string.Empty;
            if (gorselUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return gorselUrl;

            var client = CreateClientWithToken();
            var baseUrl = client.BaseAddress?.GetLeftPart(UriPartial.Authority) ?? "https://localhost:7001";

            return $"{baseUrl.TrimEnd('/')}/{gorselUrl.TrimStart('/')}";
        }
        private async Task KategorileriViewBageYukleAsync()
        {
            try
            {
                var client = CreateClientWithToken();
                var response = await client.GetAsync("kategoriler");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var kategoriler = JsonSerializer.Deserialize<List<KategoriListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<KategoriListeDto>();

                    // Ürünlerin ana kategorisi yalnızca Header menüsündeki altı sabit başlıktan seçilir.
                    var anaMenuKategorileri = kategoriler
                        .Where(k => k.UstKategoriId == null && AnaMenuSeoUrlListesi.Contains(k.SeoUrl))
                        .OrderBy(k => k.Ad)
                        .ToList();

                    var anaKategoriler = anaMenuKategorileri
                        .Select(k => new SelectListItem
                        {
                            Value = k.Id.ToString(),
                            Text = k.Ad
                        }).ToList();

                    ViewBag.AnaKategoriler = anaKategoriler;

                    // JS ile dinamik alt kategori eşleştirmesi için hafif JSON
                    var jsonListe = kategoriler.Select(k => new
                    {
                        id = k.Id.ToString(),
                        ad = k.Ad,
                        ustKategoriId = k.UstKategoriId?.ToString()
                    }).ToList();

                    ViewBag.TumKategorilerJson = JsonSerializer.Serialize(jsonListe);

                    // Geriye dönük uyumluluk için hiyerarşik liste
                    var hiyerarsikListe = anaMenuKategorileri
                        .SelectMany(ana => new[] { ana }.Concat(
                            kategoriler
                                .Where(alt => alt.UstKategoriId == ana.Id)
                                .OrderBy(alt => alt.Ad)
                        ))
                        .Select(k => new SelectListItem
                        {
                            Value = k.Id.ToString(),
                            Text = k.UstKategoriId == null ? k.Ad : $"- {k.Ad}"
                        }).ToList();

                    ViewBag.Kategoriler = new SelectList(hiyerarsikListe, "Value", "Text");
                }
                else
                {
                    ViewBag.AnaKategoriler = new List<SelectListItem>();
                    ViewBag.TumKategorilerJson = "[]";
                    ViewBag.Kategoriler = new SelectList(new List<SelectListItem>(), "Value", "Text");
                }
            }
            catch (Exception)
            {
                ViewBag.AnaKategoriler = new List<SelectListItem>();
                ViewBag.TumKategorilerJson = "[]";
                ViewBag.Kategoriler = new SelectList(new List<SelectListItem>(), "Value", "Text");
            }
        }

        [HttpGet("admin/kategoriler-json")]
        public async Task<IActionResult> KategorilerJson()
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync("kategoriler");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = CreateClientWithToken();

            // API tarafındaki rotamız: api/admin/urunler
            var response = await client.GetAsync("admin/urunler");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var urunler = JsonSerializer.Deserialize<List<UrunListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (urunler != null)
                {
                    foreach (var urun in urunler)
                    {
                        urun.GorselUrl = GetFullImageUrl(urun.GorselUrl);
                    }
                }

                return View(urunler ?? new List<UrunListeDto>());
            }

            return View(new List<UrunListeDto>());
        }

        [HttpGet]
        public async Task<IActionResult> Ekle()
        {
            await KategorileriViewBageYukleAsync();
            return View(new UrunEkleDto());
        }

        [HttpPost]
        public async Task<IActionResult> Ekle(UrunEkleDto model, List<IFormFile>? gorselDosyalari, IFormFile? gorselDosyasi)
        {
            ModelState.Remove("GorselUrl");

            var secilenDosyalar = (gorselDosyalari ?? new List<IFormFile>())
                .Where(f => f != null && f.Length > 0)
                .ToList();

            if (secilenDosyalar.Count == 0 && gorselDosyasi != null && gorselDosyasi.Length > 0)
            {
                secilenDosyalar.Add(gorselDosyasi);
            }

            if (secilenDosyalar.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Ürün için en az 1 adet fotoğraf seçmelisiniz.");
                await KategorileriViewBageYukleAsync();
                return View(model);
            }

            if (secilenDosyalar.Count > 5)
            {
                ModelState.AddModelError(string.Empty, "Ürün için en fazla 5 adet fotoğraf yükleyebilirsiniz.");
                await KategorileriViewBageYukleAsync();
                return View(model);
            }

            model.EkGorseller = new List<string>();
            for (int i = 0; i < secilenDosyalar.Count; i++)
            {
                var yuklenenYol = await ApiyeGorselYukleAsync(secilenDosyalar[i], "urunler");
                if (!string.IsNullOrWhiteSpace(yuklenenYol))
                {
                    if (i == 0)
                    {
                        model.GorselUrl = yuklenenYol;
                    }
                    else
                    {
                        model.EkGorseller.Add(yuklenenYol);
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                await KategorileriViewBageYukleAsync();
                return View(model);
            }

            var client = CreateClientWithToken();
            var response = await client.PostAsJsonAsync("admin/urunler", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Ürün başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Ürün eklenirken arka planda bir hata oluştu.");
            await KategorileriViewBageYukleAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Guncelle(Guid id)
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync($"admin/urunler/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var urun = JsonSerializer.Deserialize<UrunGuncelleDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (urun != null)
                {
                    urun.GorselUrl = GetFullImageUrl(urun.GorselUrl);
                    if (urun.EkGorseller != null)
                    {
                        for (int i = 0; i < urun.EkGorseller.Count; i++)
                        {
                            urun.EkGorseller[i] = GetFullImageUrl(urun.EkGorseller[i]);
                        }
                    }
                    await KategorileriViewBageYukleAsync();
                    return View(urun);
                }
            }

            TempData["HataMesaji"] = "Güncellenmek istenen ürün bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Guncelle(UrunGuncelleDto model, List<IFormFile>? gorselDosyalari, IFormFile? gorselDosyasi)
        {
            ModelState.Remove("GorselUrl");

            var secilenDosyalar = (gorselDosyalari ?? new List<IFormFile>())
                .Where(f => f != null && f.Length > 0)
                .ToList();

            if (secilenDosyalar.Count == 0 && gorselDosyasi != null && gorselDosyasi.Length > 0)
            {
                secilenDosyalar.Add(gorselDosyasi);
            }

            var clientCheck = CreateClientWithToken();
            var resCheck = await clientCheck.GetAsync($"admin/urunler/{model.Id}");
            UrunGuncelleDto? eskiUrun = null;
            if (resCheck.IsSuccessStatusCode)
            {
                var contentCheck = await resCheck.Content.ReadAsStringAsync();
                eskiUrun = JsonSerializer.Deserialize<UrunGuncelleDto>(contentCheck, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            if (secilenDosyalar.Count > 0)
            {
                if (secilenDosyalar.Count > 5)
                {
                    ModelState.AddModelError(string.Empty, "Ürün için en fazla 5 adet fotoğraf yükleyebilirsiniz.");
                    await KategorileriViewBageYukleAsync();
                    return View(model);
                }

                model.EkGorseller = new List<string>();
                for (int i = 0; i < secilenDosyalar.Count; i++)
                {
                    var yuklenenYol = await ApiyeGorselYukleAsync(secilenDosyalar[i], "urunler");
                    if (!string.IsNullOrWhiteSpace(yuklenenYol))
                    {
                        if (i == 0)
                        {
                            model.GorselUrl = yuklenenYol;
                        }
                        else
                        {
                            model.EkGorseller.Add(yuklenenYol);
                        }
                    }
                }
            }
            else
            {
                if (eskiUrun != null)
                {
                    if (string.IsNullOrWhiteSpace(model.GorselUrl))
                    {
                        model.GorselUrl = eskiUrun.GorselUrl;
                    }
                    if (model.EkGorseller == null || !model.EkGorseller.Any())
                    {
                        model.EkGorseller = eskiUrun.EkGorseller ?? new List<string>();
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                await KategorileriViewBageYukleAsync();
                return View(model);
            }

            var client = CreateClientWithToken();
            var response = await client.PutAsJsonAsync("admin/urunler", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Ürün başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Ürün güncellenirken bir hata oluştu.");
            await KategorileriViewBageYukleAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Detay(Guid id)
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync($"admin/urunler/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var urun = JsonSerializer.Deserialize<UrunGuncelleDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (urun != null)
                {
                    // Görsel yolunu API tabanlı tam URL formatına dönüştürüyoruz
                    urun.GorselUrl = GetFullImageUrl(urun.GorselUrl);

                    // Kategori ismini ekrana yansıtmak için API'den kategorileri çekiyoruz
                    var catResponse = await client.GetAsync("kategoriler");
                    if (catResponse.IsSuccessStatusCode)
                    {
                        var catContent = await catResponse.Content.ReadAsStringAsync();
                        var kategoriler = JsonSerializer.Deserialize<List<KategoriListeDto>>(catContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        ViewBag.KategoriAdi = kategoriler?.FirstOrDefault(x => x.Id == urun.KategoriId)?.Ad ?? "Kategori Belirtilmemiş";
                    }
                    else
                    {
                        ViewBag.KategoriAdi = "Kategori Belirtilmemiş";
                    }

                    return View(urun);
                }
            }

            TempData["HataMesaji"] = "İncelenmek istenen ürün bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Sil(Guid id)
        {
            var client = CreateClientWithToken();
            var response = await client.DeleteAsync($"admin/urunler/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Ürün başarıyla silindi.";
            }
            else
            {
                TempData["HataMesaji"] = "Ürün silinirken bir hata oluştu. Siparişlerle veya varyantlarla bağlantılı olabilir.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AiAciklamaUret([FromBody] ECommerce.Application.DTOs.Urun.AiAciklamaIstekDto model)
        {
            var client = CreateClientWithToken();
            var response = await client.PostAsJsonAsync("admin/urunler/ai-aciklama-uret", model);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                return Content(jsonContent, "application/json");
            }

            return BadRequest(new { Mesaj = "AI servisi yanıt vermedi." });
        }


        [HttpPost]
        public async Task<IActionResult> HizliDurumDegistir(Guid id, bool aktifMi)
        {
            var client = CreateClientWithToken();
            // API tarafında sadece durumu güncelleyen bir endpoint'e istek atıyoruz (Örnek: PUT api/admin/urunler/durum-degistir)
            var response = await client.PutAsJsonAsync($"admin/urunler/durum-degistir/{id}", new { AktifMi = aktifMi });

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, mesaj = "Durum güncellendi." });
            }

            return Json(new { success = false, mesaj = "Durum güncellenirken bir hata oluştu." });
        }
    }
}
