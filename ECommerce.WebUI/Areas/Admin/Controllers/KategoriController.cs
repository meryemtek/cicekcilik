using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Text.Json;
using ECommerce.Application.DTOs.Kategori;
using ECommerce.Application.DTOs.Urun;

namespace ECommerce.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class KategoriController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IWebHostEnvironment _env; // WebUI wwwroot erişimi için eklendi

    public KategoriController(IHttpClientFactory httpClientFactory, IWebHostEnvironment env)
    {
        _httpClientFactory = httpClientFactory;
        _env = env;
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

    private async Task KategorileriViewBageYukleAsync(Guid? haricTutulacakId = null)
    {
        var client = CreateClientWithToken();
        var response = await client.GetAsync("kategoriler");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var kategoriler = JsonSerializer.Deserialize<List<KategoriListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var anaKategoriler = kategoriler?
                .Where(x => x.UstKategoriId == null && x.Id != haricTutulacakId)
                .OrderBy(x => x.Ad)
                .ToList() ?? new List<KategoriListeDto>();

            ViewBag.UstKategoriler = new SelectList(anaKategoriler, "Id", "Ad");
        }
        else
        {
            ViewBag.UstKategoriler = new SelectList(new List<KategoriListeDto>(), "Id", "Ad");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var client = CreateClientWithToken();
        var response = await client.GetAsync("kategoriler");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var kategoriler = JsonSerializer.Deserialize<List<KategoriListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(kategoriler);
        }

        return View(new List<KategoriListeDto>());
    }

    [HttpGet]
    public async Task<IActionResult> Ekle()
    {
        await KategorileriViewBageYukleAsync();
        return View(new KategoriEkleDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ekle(KategoriEkleDto model, IFormFile? gorselDosyasi)
    {
        ModelState.Remove("GorselUrl");

        if (!ModelState.IsValid)
        {
            await KategorileriViewBageYukleAsync();
            return View(model);
        }

        // Slider'daki gibi doğrudan WebUI wwwroot klasörüne kaydediyoruz
        if (gorselDosyasi != null && gorselDosyasi.Length > 0)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "kategoriler");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var benzersizAd = Guid.NewGuid().ToString() + "_" + Path.GetFileName(gorselDosyasi.FileName);
            var dosyaYolu = Path.Combine(uploadsFolder, benzersizAd);

            using (var stream = new FileStream(dosyaYolu, FileMode.Create))
            {
                await gorselDosyasi.CopyToAsync(stream);
            }

            model.GorselUrl = $"/uploads/kategoriler/{benzersizAd}";
        }

        var client = CreateClientWithToken();
        var response = await client.PostAsJsonAsync("kategoriler", model);

        if (response.IsSuccessStatusCode)
        {
            TempData["BasariliMesaj"] = "Kategori başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Kategori eklenirken arka planda bir hata oluştu.");
        await KategorileriViewBageYukleAsync();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Guncelle(Guid id)
    {
        var client = CreateClientWithToken();
        var response = await client.GetAsync($"kategoriler/{id}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var kategori = JsonSerializer.Deserialize<KategoriGuncelleDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (kategori != null)
            {
                await KategorileriViewBageYukleAsync(id);
                return View(kategori);
            }
        }

        TempData["HataMesaji"] = "Güncellenmek istenen kategori bulunamadı.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Guncelle(KategoriGuncelleDto model, IFormFile? gorselDosyasi)
    {
        ModelState.Remove("GorselUrl");

        if (!ModelState.IsValid)
        {
            await KategorileriViewBageYukleAsync(model.Id);
            return View(model);
        }

        // Yeni görsel seçildiyse WebUI wwwroot klasörüne kaydediyoruz
        if (gorselDosyasi != null && gorselDosyasi.Length > 0)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "kategoriler");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var benzersizAd = Guid.NewGuid().ToString() + "_" + Path.GetFileName(gorselDosyasi.FileName);
            var dosyaYolu = Path.Combine(uploadsFolder, benzersizAd);

            using (var stream = new FileStream(dosyaYolu, FileMode.Create))
            {
                await gorselDosyasi.CopyToAsync(stream);
            }

            model.GorselUrl = $"/uploads/kategoriler/{benzersizAd}";
        }

        var client = CreateClientWithToken();
        var response = await client.PutAsJsonAsync("kategoriler", model);

        if (response.IsSuccessStatusCode)
        {
            TempData["BasariliMesaj"] = "Kategori başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Kategori güncellenirken bir hata oluştu.");
        await KategorileriViewBageYukleAsync(model.Id);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sil(Guid id)
    {
        var client = CreateClientWithToken();
        var response = await client.DeleteAsync($"kategoriler/{id}");

        if (response.IsSuccessStatusCode)
        {
            TempData["BasariliMesaj"] = "Kategori başarıyla silindi.";
        }
        else
        {
            TempData["HataMesaji"] = "Kategori silinirken bir hata oluştu.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DurumDegistir(Guid id)
    {
        var client = CreateClientWithToken();
        var response = await client.GetAsync($"kategoriler/{id}");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var kategori = JsonSerializer.Deserialize<KategoriGuncelleDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (kategori != null)
            {
                kategori.AktifMi = !kategori.AktifMi;
                var putResponse = await client.PutAsJsonAsync("kategoriler", kategori);
                if (putResponse.IsSuccessStatusCode)
                {
                    TempData["BasariliMesaj"] = kategori.AktifMi 
                        ? $"'{kategori.Ad}' kategorisi başarıyla aktife alındı." 
                        : $"'{kategori.Ad}' kategorisi arşivlendi (pasife alındı).";
                    return RedirectToAction(nameof(Index));
                }
            }
        }

        TempData["HataMesaji"] = "Kategori durumu değiştirilirken bir hata oluştu.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Detay(Guid id)
    {
        var client = CreateClientWithToken();

        var katResponse = await client.GetAsync($"kategoriler/{id}");
        if (!katResponse.IsSuccessStatusCode)
        {
            TempData["HataMesaji"] = "İncelenmek istenen kategori bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        var katContent = await katResponse.Content.ReadAsStringAsync();
        var kategori = JsonSerializer.Deserialize<KategoriGuncelleDto>(katContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var urunResponse = await client.GetAsync("admin/urunler");
        var kategoriUrunleri = new List<UrunListeDto>();

        if (urunResponse.IsSuccessStatusCode)
        {
            var urunContent = await urunResponse.Content.ReadAsStringAsync();
            var tumUrunler = JsonSerializer.Deserialize<List<UrunListeDto>>(urunContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (tumUrunler != null)
            {
                kategoriUrunleri = tumUrunler.Where(x => x.KategoriId == id).ToList();
            }
        }

        ViewBag.KategoriUrunleri = kategoriUrunleri;
        return View(kategori);
    }
}
