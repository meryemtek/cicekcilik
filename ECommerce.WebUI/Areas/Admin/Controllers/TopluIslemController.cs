using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using ECommerce.Application.DTOs.Urun;
using ECommerce.Application.DTOs.Kategori;

namespace ECommerce.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class TopluIslemController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public TopluIslemController(IHttpClientFactory httpClientFactory)
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

    private async Task KategorileriViewBageYukleAsync()
    {
        var client = CreateClientWithToken();
        var response = await client.GetAsync("kategoriler");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var kategoriler = JsonSerializer.Deserialize<List<KategoriListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            ViewBag.Kategoriler = new SelectList(kategoriler, "Id", "Ad");
        }
        else
        {
            ViewBag.Kategoriler = new SelectList(new List<KategoriListeDto>(), "Id", "Ad");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        await KategorileriViewBageYukleAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> FiyatGuncelle(TopluFiyatGuncelleDto dto)
    {
        var client = CreateClientWithToken();
        var response = await client.PostAsJsonAsync("admin/toplu-urun/fiyat-guncelle", dto);

        if (response.IsSuccessStatusCode)
        {
            TempData["BasariliMesaj"] = "Ürün fiyatları başarıyla güncellendi.";
        }
        else
        {
            TempData["HataMesaji"] = "Fiyatlar güncellenirken sunucu tarafında bir hata oluştu.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DurumDegistir(bool aktifMi, Guid? kategoriId)
    {
        var client = CreateClientWithToken();

        var url = $"admin/toplu-urun/durum-degistir?aktifMi={aktifMi}";
        if (kategoriId.HasValue)
        {
            url += $"&kategoriId={kategoriId.Value}";
        }

        var response = await client.PostAsync(url, null); // Post işlemi ancak body boş

        if (response.IsSuccessStatusCode)
        {
            TempData["BasariliMesaj"] = $"Seçilen ürünlerin durumu başarıyla {(aktifMi ? "satışa açıldı" : "satışa kapatıldı")}.";
        }
        else
        {
            TempData["HataMesaji"] = "Ürün durumları değiştirilirken bir hata oluştu.";
        }

        return RedirectToAction(nameof(Index));
    }
}