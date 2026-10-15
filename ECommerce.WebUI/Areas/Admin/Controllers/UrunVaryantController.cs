using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using ECommerce.Application.DTOs.UrunVaryant;
using ECommerce.Application.DTOs.Urun;

namespace ECommerce.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class UrunVaryantController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public UrunVaryantController(IHttpClientFactory httpClientFactory)
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
    public async Task<IActionResult> Index(Guid urunId)
    {
        var client = CreateClientWithToken();

        // Ürün bilgilerini sayfada başlık olarak göstermek için çekiyoruz
        var urunResponse = await client.GetAsync($"admin/urunler/{urunId}");
        if (urunResponse.IsSuccessStatusCode)
        {
            var urunContent = await urunResponse.Content.ReadAsStringAsync();
            var urun = JsonSerializer.Deserialize<UrunGuncelleDto>(urunContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            ViewBag.UrunAdi = urun?.Ad;
        }

        ViewBag.UrunId = urunId;

        // İlgili ürüne ait varyantları çekiyoruz
        var response = await client.GetAsync($"admin/urun-varyantlari/urun/{urunId}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var varyantlar = JsonSerializer.Deserialize<List<UrunVaryantListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(varyantlar);
        }

        return View(new List<UrunVaryantListeDto>());
    }

    [HttpGet]
    public IActionResult Ekle(Guid urunId)
    {
        ViewBag.UrunId = urunId;
        return View(new UrunVaryantEkleDto { UrunId = urunId });
    }

    [HttpPost]
    public async Task<IActionResult> Ekle(UrunVaryantEkleDto model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.UrunId = model.UrunId;
            return View(model);
        }

        var client = CreateClientWithToken();
        var response = await client.PostAsJsonAsync("admin/urun-varyantlari", model);

        if (response.IsSuccessStatusCode)
        {
            TempData["BasariliMesaj"] = "Ürün varyantı başarıyla eklendi.";
            return RedirectToAction(nameof(Index), new { urunId = model.UrunId });
        }

        ModelState.AddModelError(string.Empty, "Varyant eklenirken arka planda bir hata oluştu.");
        ViewBag.UrunId = model.UrunId;
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Guncelle(Guid id, Guid urunId)
    {
        ViewBag.UrunId = urunId;
        var client = CreateClientWithToken();

        // Tekil varyant endpoint'i olmadığı için listeyi çekip içinden buluyoruz
        var response = await client.GetAsync($"admin/urun-varyantlari/urun/{urunId}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var varyantlar = JsonSerializer.Deserialize<List<UrunVaryantGuncelleDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var varyant = varyantlar?.FirstOrDefault(x => x.Id == id);
            if (varyant != null)
            {
                return View(varyant);
            }
        }

        TempData["HataMesaji"] = "Güncellenmek istenen varyant bulunamadı.";
        return RedirectToAction(nameof(Index), new { urunId = urunId });
    }

    [HttpPost]
    public async Task<IActionResult> Guncelle(UrunVaryantGuncelleDto model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.UrunId = model.UrunId;
            return View(model);
        }

        var client = CreateClientWithToken();
        var response = await client.PutAsJsonAsync("admin/urun-varyantlari", model);

        if (response.IsSuccessStatusCode)
        {
            TempData["BasariliMesaj"] = "Ürün varyantı başarıyla güncellendi.";
            return RedirectToAction(nameof(Index), new { urunId = model.UrunId });
        }

        ModelState.AddModelError(string.Empty, "Varyant güncellenirken bir hata oluştu.");
        ViewBag.UrunId = model.UrunId;
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Sil(Guid id, Guid urunId)
    {
        var client = CreateClientWithToken();
        var response = await client.DeleteAsync($"admin/urun-varyantlari/{id}");

        if (response.IsSuccessStatusCode)
        {
            TempData["BasariliMesaj"] = "Ürün varyantı başarıyla silindi.";
        }
        else
        {
            TempData["HataMesaji"] = "Varyant silinirken bir hata oluştu.";
        }

        return RedirectToAction(nameof(Index), new { urunId = urunId });
    }
}