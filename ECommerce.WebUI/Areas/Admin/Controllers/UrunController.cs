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
            var client = CreateClientWithToken();
            var response = await client.GetAsync("kategoriler");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var kategoriler = JsonSerializer.Deserialize<List<KategoriListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (kategoriler != null && kategoriler.Any())
                {
                    // 1. Önce ana kategorileri (UstKategoriId == null olanları) alfabetik sırala
                    // 2. Ardından her ana kategorinin kendi alt kategorilerini hemen onun altına alfabetik olarak ekle
                    var hiyerarsikListe = kategoriler
                        .Where(k => k.UstKategoriId == null)
                        .OrderBy(k => k.Ad)
                        .SelectMany(ana => new[] { ana }.Concat(
                            kategoriler
                                .Where(alt => alt.UstKategoriId == ana.Id)
                                .OrderBy(alt => alt.Ad)
                        ))
                        .Select(k => new SelectListItem
                        {
                            Value = k.Id.ToString(),
                            // Eğer alt kategori ise başına "- " koyuyoruz ki Select2 bunu Javascript ile yakalayıp CSS basabilsin.
                            Text = k.UstKategoriId == null ? k.Ad : $"- {k.Ad}"
                        }).ToList();

                    // Model listesi yerine SelectListItem listesi verdiğimiz için DataValueField "Value", DataTextField "Text" oldu.
                    ViewBag.Kategoriler = new SelectList(hiyerarsikListe, "Value", "Text");
                }
                else
                {
                    ViewBag.Kategoriler = new SelectList(new List<SelectListItem>(), "Value", "Text");
                }
            }
            else
            {
                ViewBag.Kategoriler = new SelectList(new List<SelectListItem>(), "Value", "Text");
            }
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
        public async Task<IActionResult> Ekle(UrunEkleDto model, IFormFile? gorselDosyasi)
        {
            ModelState.Remove("GorselUrl");

            if (!ModelState.IsValid)
            {
                await KategorileriViewBageYukleAsync();
                return View(model);
            }

            if (gorselDosyasi != null)
            {
                // GÜNCELLENDİ: Artık doğrudan API'ye upload işlemi yapılıyor
                model.GorselUrl = await ApiyeGorselYukleAsync(gorselDosyasi, "urunler");
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
                    await KategorileriViewBageYukleAsync();
                    return View(urun);
                }
            }

            TempData["HataMesaji"] = "Güncellenmek istenen ürün bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Guncelle(UrunGuncelleDto model, IFormFile? gorselDosyasi)
        {
            ModelState.Remove("GorselUrl");

            // EĞER YENİ BİR GÖRSEL SEÇİLMEDİYSE
            if (gorselDosyasi == null || gorselDosyasi.Length == 0)
            {
                // Veritabanındaki/istemciden gelen mevcut görsel yolunu kaybetmemek için 
                // Eğer hidden input boş geldiyse, API'den o anki eski ürünü çekip görselini koruyabiliriz:
                if (string.IsNullOrWhiteSpace(model.GorselUrl))
                {
                    var clientCheck = CreateClientWithToken();
                    var resCheck = await clientCheck.GetAsync($"admin/urunler/{model.Id}");
                    if (resCheck.IsSuccessStatusCode)
                    {
                        var contentCheck = await resCheck.Content.ReadAsStringAsync();
                        var eskiUrun = JsonSerializer.Deserialize<UrunGuncelleDto>(contentCheck, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (eskiUrun != null)
                        {
                            model.GorselUrl = eskiUrun.GorselUrl; // Eski görsel yolunu koruyoruz!
                        }
                    }
                }
            }
            else
            {
                // Yeni görsel yüklenmişse API'ye yükletip yeni yolu atıyoruz
                model.GorselUrl = await ApiyeGorselYukleAsync(gorselDosyasi, "urunler");
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

        [HttpGet]
        public IActionResult XmlImport()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> XmlImport(IFormFile xmlDosyasi)
        {
            if (xmlDosyasi == null || xmlDosyasi.Length == 0)
            {
                TempData["HataMesaji"] = "Lütfen içe aktarmak için geçerli bir XML dosyası seçin.";
                return View();
            }

            var client = CreateClientWithToken();

            using var content = new MultipartFormDataContent();
            using var fileStream = xmlDosyasi.OpenReadStream();
            using var streamContent = new StreamContent(fileStream);

            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
            content.Add(streamContent, "xmlDosyasi", xmlDosyasi.FileName);

            var response = await client.PostAsync("admin/xml-import/yukle", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "XML dosyasındaki tüm ürünler başarıyla sisteme aktarıldı.";
                return RedirectToAction(nameof(Index));
            }

            TempData["HataMesaji"] = "XML dosyası işlenirken arka planda bir hata oluştu.";
            return View();
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