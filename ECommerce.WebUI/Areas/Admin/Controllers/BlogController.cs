using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using ECommerce.Application.DTOs.Blog;

namespace ECommerce.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class BlogController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public BlogController(IHttpClientFactory httpClientFactory)
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

    // Kapak görseli ve genel yüklemeler için
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
            using var doc = JsonDocument.Parse(responseContent);
            var root = doc.RootElement;

            if (root.TryGetProperty("url", out var urlProp) || root.TryGetProperty("Url", out urlProp))
            {
                return urlProp.GetString() ?? string.Empty;
            }
        }

        return string.Empty;
    }

    // YENİ: CKEditor gibi Zengin Metin Editörleri içinden yapılan görsel yüklemelerini karşılar
    [HttpPost]
    public async Task<IActionResult> UploadEditorImage(IFormFile upload)
    {
        if (upload != null && upload.Length > 0)
        {
            // Editörden gelen görselleri "blog/icerik" alt klasörüne kaydediyoruz
            var imageUrl = await ApiyeGorselYukleAsync(upload, "blog/icerik");

            if (!string.IsNullOrEmpty(imageUrl))
            {
                // Zengin Metin Editörlerinin (örneğin CKEditor 5) beklediği standart JSON yanıtı
                return Json(new { uploaded = true, url = imageUrl });
            }
        }

        // Hata durumunda editöre gönderilecek JSON formatı
        return Json(new { uploaded = false, error = new { message = "Görsel sunucuya yüklenirken bir hata oluştu." } });
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var client = CreateClientWithToken();
        var response = await client.GetAsync("admin/blog");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var yazilar = JsonSerializer.Deserialize<List<BlogListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Eğer resim yollarını baseUrl ile birleştirmen gerekirse burada yapabilirsin
            if (yazilar != null)
            {
                var baseUrl = client.BaseAddress?.GetLeftPart(UriPartial.Authority) ?? "https://localhost:7001";
                foreach (var item in yazilar)
                {
                    if (!string.IsNullOrWhiteSpace(item.GorselUrl) && !item.GorselUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    {
                        item.GorselUrl = $"{baseUrl.TrimEnd('/')}/{item.GorselUrl.TrimStart('/')}";
                    }
                }
            }

            return View(yazilar ?? new List<BlogListeDto>());
        }

        return View(new List<BlogListeDto>());
    }

    [HttpGet]
    public IActionResult Ekle()
    {
        return View(new BlogEkleDto { YayinlanmaTarihi = DateTime.Now }); // Varsayılan tarihi şu an olarak atadık
    }

    [HttpPost]
    public async Task<IActionResult> Ekle(BlogEkleDto model, IFormFile? gorselDosyasi)
    {
        ModelState.Remove("GorselUrl");

        if (!ModelState.IsValid)
            return View(model);

        if (gorselDosyasi != null)
        {
            model.GorselUrl = await ApiyeGorselYukleAsync(gorselDosyasi, "blog");
        }

        var client = CreateClientWithToken();
        var response = await client.PostAsJsonAsync("admin/blog", model);

        if (response.IsSuccessStatusCode)
        {
            TempData["BasariliMesaj"] = "Blog yazısı başarıyla yayınlandı.";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Blog yazısı eklenirken bir hata oluştu.");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Guncelle(Guid id)
    {
        var client = CreateClientWithToken();
        var response = await client.GetAsync($"admin/blog/{id}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var yazi = JsonSerializer.Deserialize<BlogGuncelleDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (yazi != null)
            {
                // Güncelleme sayfasında eski görseli göstermek istersen ViewBag ile taşıyabilirsin
                ViewBag.MevcutGorsel = yazi.GorselUrl;
                return View(yazi);
            }
        }

        TempData["HataMesaji"] = "Güncellenmek istenen blog yazısı bulunamadı.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Guncelle(BlogGuncelleDto model, IFormFile? gorselDosyasi)
    {
        ModelState.Remove("GorselUrl");

        if (!ModelState.IsValid)
            return View(model);

        if (gorselDosyasi != null)
        {
            model.GorselUrl = await ApiyeGorselYukleAsync(gorselDosyasi, "blog");
        }

        var client = CreateClientWithToken();
        var response = await client.PutAsJsonAsync("admin/blog", model);

        if (response.IsSuccessStatusCode)
        {
            TempData["BasariliMesaj"] = "Blog yazısı başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Blog yazısı güncellenirken bir hata oluştu.");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Detay(Guid id)
    {
        var client = CreateClientWithToken();
        var response = await client.GetAsync($"admin/blog/{id}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var yazi = JsonSerializer.Deserialize<BlogGuncelleDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (yazi != null)
            {
                // Görsel linkini tam URL'e çevirme işlemi (Index'te yaptığımız gibi)
                var baseUrl = client.BaseAddress?.GetLeftPart(UriPartial.Authority) ?? "https://localhost:7001";
                if (!string.IsNullOrWhiteSpace(yazi.GorselUrl) && !yazi.GorselUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    yazi.GorselUrl = $"{baseUrl.TrimEnd('/')}/{yazi.GorselUrl.TrimStart('/')}";
                }

                return View(yazi); // Admin için salt okunur detay (Önizleme) sayfası
            }
        }

        TempData["HataMesaji"] = "Görüntülenmek istenen blog yazısı bulunamadı.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Sil(Guid id)
    {
        var client = CreateClientWithToken();
        var response = await client.DeleteAsync($"admin/blog/{id}");

        if (response.IsSuccessStatusCode)
        {
            TempData["BasariliMesaj"] = "Blog yazısı başarıyla silindi.";
        }
        else
        {
            TempData["HataMesaji"] = "Blog yazısı silinirken bir hata oluştu.";
        }

        return RedirectToAction(nameof(Index));
    }
}