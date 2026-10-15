using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using ECommerce.Application.DTOs.IletisimMesaji; // DTO'larını ana DTO klasörüne koyduğunu varsayıyorum, değilse klasör adını eklemelisin.

namespace ECommerce.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class IletisimMesajiController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public IletisimMesajiController(IHttpClientFactory httpClientFactory)
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
        var response = await client.GetAsync("admin/iletisim-mesajlari");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var mesajlar = JsonSerializer.Deserialize<List<IletisimMesajiListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(mesajlar);
        }

        return View(new List<IletisimMesajiListeDto>());
    }

    [HttpGet]
    public async Task<IActionResult> Detay(Guid id)
    {
        var client = CreateClientWithToken();
        var response = await client.GetAsync($"admin/iletisim-mesajlari/{id}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var mesaj = JsonSerializer.Deserialize<IletisimMesajiDetayDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (mesaj != null)
            {
                return View(mesaj);
            }
        }

        TempData["HataMesaji"] = "Okunmak istenen iletişim mesajı bulunamadı.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Sil(Guid id)
    {
        var client = CreateClientWithToken();
        var response = await client.DeleteAsync($"admin/iletisim-mesajlari/{id}");

        if (response.IsSuccessStatusCode)
        {
            TempData["BasariliMesaj"] = "Mesaj başarıyla silindi.";
        }
        else
        {
            TempData["HataMesaji"] = "Mesaj silinirken bir hata oluştu.";
        }

        return RedirectToAction(nameof(Index));
    }
}