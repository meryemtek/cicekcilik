using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using ECommerce.Application.DTOs.BankaHesabi;

namespace ECommerce.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class BankaHesabiController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public BankaHesabiController(IHttpClientFactory httpClientFactory)
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
        var response = await client.GetAsync("admin/banka-hesaplari");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var hesaplar = JsonSerializer.Deserialize<List<BankaHesabiListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(hesaplar);
        }

        return View(new List<BankaHesabiListeDto>());
    }

    [HttpGet]
    public IActionResult Ekle()
    {
        return View(new BankaHesabiEkleDto());
    }

    [HttpPost]
    public async Task<IActionResult> Ekle(BankaHesabiEkleDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var client = CreateClientWithToken();
        var response = await client.PostAsJsonAsync("admin/banka-hesaplari", model);

        if (response.IsSuccessStatusCode)
        {
            TempData["BasariliMesaj"] = "Banka hesabı başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Banka hesabı eklenirken bir hata oluştu.");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Guncelle(Guid id)
    {
        var client = CreateClientWithToken();
        var response = await client.GetAsync($"admin/banka-hesaplari/{id}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var hesap = JsonSerializer.Deserialize<BankaHesabiGuncelleDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (hesap != null)
            {
                return View(hesap);
            }
        }

        TempData["HataMesaji"] = "Güncellenmek istenen hesap bulunamadı.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Guncelle(BankaHesabiGuncelleDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var client = CreateClientWithToken();
        var response = await client.PutAsJsonAsync("admin/banka-hesaplari", model);

        if (response.IsSuccessStatusCode)
        {
            TempData["BasariliMesaj"] = "Banka hesabı başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Hesap güncellenirken bir hata oluştu.");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Sil(Guid id)
    {
        var client = CreateClientWithToken();
        var response = await client.DeleteAsync($"admin/banka-hesaplari/{id}");

        if (response.IsSuccessStatusCode)
        {
            TempData["BasariliMesaj"] = "Banka hesabı başarıyla silindi.";
        }
        else
        {
            TempData["HataMesaji"] = "Hesap silinirken bir hata oluştu.";
        }

        return RedirectToAction(nameof(Index));
    }
}