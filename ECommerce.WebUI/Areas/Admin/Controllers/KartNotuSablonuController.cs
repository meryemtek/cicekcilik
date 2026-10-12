using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using ECommerce.Application.DTOs;
using ECommerce.Application.DTOs.KartNotuSablonu;

namespace ECommerce.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class KartNotuSablonuController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public KartNotuSablonuController(IHttpClientFactory httpClientFactory)
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

    // YENİ: Sistemdeki mevcut kart notu kategorilerini (Doğum Günü, Yıldönümü vb.) dinamik olarak getirir
    private async Task KategorileriViewBageYukleAsync()
    {
        var client = CreateClientWithToken();
        var response = await client.GetAsync("admin/kart-notlari");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var sablonlar = JsonSerializer.Deserialize<List<KartNotuSablonuListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Aynı kategori ismini tekrar tekrar listelememek için Distinct() kullanıyoruz
            var kategoriler = sablonlar?.Select(x => x.Kategori).Distinct().ToList() ?? new List<string>();
            ViewBag.Kategoriler = kategoriler;
        }
        else
        {
            ViewBag.Kategoriler = new List<string>();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var client = CreateClientWithToken();
        var response = await client.GetAsync("admin/kart-notlari");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var sablonlar = JsonSerializer.Deserialize<List<KartNotuSablonuListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(sablonlar);
        }

        return View(new List<KartNotuSablonuListeDto>());
    }

    [HttpGet]
    public async Task<IActionResult> Ekle()
    {
        await KategorileriViewBageYukleAsync();
        return View(new KartNotuSablonuEkleDto { SiraNo = 1, AktifMi = true });
    }

    [HttpPost]
    public async Task<IActionResult> Ekle(KartNotuSablonuEkleDto model)
    {
        if (!ModelState.IsValid)
        {
            await KategorileriViewBageYukleAsync();
            return View(model);
        }

        var client = CreateClientWithToken();
        var response = await client.PostAsJsonAsync("admin/kart-notlari", model);

        if (response.IsSuccessStatusCode)
        {
            TempData["BasariliMesaj"] = "Hazır kart notu başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Not eklenirken bir hata oluştu.");
        await KategorileriViewBageYukleAsync();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Guncelle(Guid id)
    {
        var client = CreateClientWithToken();
        var response = await client.GetAsync($"admin/kart-notlari/{id}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var sablon = JsonSerializer.Deserialize<KartNotuSablonuGuncelleDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (sablon != null)
            {
                await KategorileriViewBageYukleAsync();
                return View(sablon);
            }
        }

        TempData["HataMesaji"] = "Güncellenmek istenen kart notu bulunamadı.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Guncelle(KartNotuSablonuGuncelleDto model)
    {
        if (!ModelState.IsValid)
        {
            await KategorileriViewBageYukleAsync();
            return View(model);
        }

        var client = CreateClientWithToken();
        var response = await client.PutAsJsonAsync("admin/kart-notlari", model);

        if (response.IsSuccessStatusCode)
        {
            TempData["BasariliMesaj"] = "Hazır kart notu başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Not güncellenirken bir hata oluştu.");
        await KategorileriViewBageYukleAsync();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Sil(Guid id)
    {
        var client = CreateClientWithToken();
        var response = await client.DeleteAsync($"admin/kart-notlari/{id}");

        if (response.IsSuccessStatusCode)
        {
            TempData["BasariliMesaj"] = "Hazır kart notu başarıyla silindi.";
        }
        else
        {
            TempData["HataMesaji"] = "Kart notu silinirken bir hata oluştu.";
        }

        return RedirectToAction(nameof(Index));
    }
}