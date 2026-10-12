using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;
using ECommerce.Application.DTOs.Sayfa;
using ECommerce.WebUI.Models;

namespace ECommerce.WebUI.ViewComponents;

public class SayfaNavViewComponent : ViewComponent
{
    private readonly IHttpClientFactory _httpClientFactory;

    public SayfaNavViewComponent(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        // 1. Aktif sayfaları çekiyoruz
        var sayfaResponse = await client.GetAsync("sayfalar/aktif-liste");
        var sayfalar = new List<SayfaListeDto>();

        if (sayfaResponse.IsSuccessStatusCode)
        {
            sayfalar = await sayfaResponse.Content.ReadFromJsonAsync<List<SayfaListeDto>>() ?? new();
        }

        // 2. Admin Panelindeki Site Ayarlarından "Telefon 1" Bilgisini Çekme
        string telefonNumarasi = "0 534 526 85 50"; // Varsayılan değer
        try
        {
            var ayarResponse = await client.GetAsync("siteayar");
            if (ayarResponse.IsSuccessStatusCode)
            {
                using var stream = await ayarResponse.Content.ReadAsStreamAsync();
                using var doc = await JsonDocument.ParseAsync(stream);
                var root = doc.RootElement;

                // JSON içerisindeki olası tüm telefon alanı yazılışlarını kontrol ediyoruz
                string[] olasiAlanlar = { "telefon1", "Telefon1", "telefon", "Telefon", "phone", "Phone" };

                foreach (var alan in olasiAlanlar)
                {
                    if (root.TryGetProperty(alan, out var telProp) && telProp.ValueKind == JsonValueKind.String)
                    {
                        var deger = telProp.GetString();
                        if (!string.IsNullOrWhiteSpace(deger))
                        {
                            telefonNumarasi = deger;
                            break;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Hata durumunda konsola yazdırılır, sistem varsayılan numarayı kullanmaya devam eder
            System.Diagnostics.Debug.WriteLine("Site ayarları telefon bilgisi çekilemedi: " + ex.Message);
        }

        var model = new SayfaNavViewModel
        {
            Sayfalar = sayfalar,
            DestekTelefonu = telefonNumarasi
        };

        return View(model);
    }
}