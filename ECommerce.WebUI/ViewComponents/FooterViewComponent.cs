using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ECommerce.WebUI.Models;

namespace ECommerce.WebUI.ViewComponents;

public class FooterViewComponent : ViewComponent
{
    private readonly IHttpClientFactory _httpClientFactory;

    public FooterViewComponent(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var model = new FooterViewModel();

        try
        {
            var ayarResponse = await client.GetAsync("siteayarlari/site-ayarlari");

            if (ayarResponse.IsSuccessStatusCode)
            {
                using var stream = await ayarResponse.Content.ReadAsStreamAsync();
                using var doc = await JsonDocument.ParseAsync(stream);
                var root = doc.RootElement;

                string GetVal(params string[] names)
                {
                    foreach (var name in names)
                    {
                        if (root.TryGetProperty(name, out var val) && val.ValueKind == JsonValueKind.String)
                        {
                            var str = val.GetString();
                            if (!string.IsNullOrWhiteSpace(str)) return str;
                        }
                    }
                    return "";
                }

                // Firma Adı
                string firmaAdi = GetVal("firmaAdi", "FirmaAdi");
                if (!string.IsNullOrEmpty(firmaAdi)) model.FirmaAdi = firmaAdi;

                // İletişim Bilgileri
                model.Telefon1 = GetVal("telefon1", "Telefon1");
                model.AdminWhatsAppNumarasi = GetVal("adminWhatsAppNumarasi", "AdminWhatsAppNumarasi", "AdminWhatsappNumarasi");
                model.Email = GetVal("email", "Email", "eposta", "Eposta");
                model.Adres = GetVal("adres", "Adres");

                // Sosyal Medya
                model.InstagramUrl = GetVal("instagramUrl", "InstagramUrl");
                model.FacebookUrl = GetVal("facebookUrl", "FacebookUrl");
                model.TwitterUrl = GetVal("twitterUrl", "TwitterUrl");

                // Logo URL
                string logoVal = GetVal("logoUrl", "LogoUrl", "logo", "Logo");
                if (!string.IsNullOrEmpty(logoVal))
                {
                    model.LogoUrl = logoVal;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Footer site ayarları çekilemedi: " + ex.Message);
        }

        return View(model);
    }
}