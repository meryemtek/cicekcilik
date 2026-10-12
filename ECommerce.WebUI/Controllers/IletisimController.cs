using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using ECommerce.WebUI.Models;

namespace ECommerce.WebUI.Controllers
{
    public class IletisimController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IletisimController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("iletisim")]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var model = new IletisimViewModel();

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
                        return string.Empty;
                    }

                    model.FirmaAdi = GetVal("firmaAdi", "FirmaAdi");
                    model.Adres = GetVal("adres", "Adres");
                    model.Telefon1 = GetVal("telefon1", "Telefon1");
                    model.Email = GetVal("email", "Email", "eposta", "Eposta");
                    model.InstagramUrl = GetVal("instagramUrl", "InstagramUrl");
                    model.FacebookUrl = GetVal("facebookUrl", "FacebookUrl");
                    model.TwitterUrl = GetVal("twitterUrl", "TwitterUrl");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("İletişim sayfası site ayarları alınamadı: " + ex.Message);
            }

            return View(model);
        }

        [HttpPost("iletisim")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IletisimViewModel model)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var jsonContent = new StringContent(JsonSerializer.Serialize(model.Dto, jsonOptions), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("iletisim", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Mesajınız başarıyla gönderildi. En kısa sürede sizinle iletişime geçilecektir.";
                return RedirectToAction(nameof(Index));
            }

            TempData["HataMesaji"] = "Mesaj gönderilirken bir hata oluştu. Lütfen tüm alanları kontrol edip tekrar deneyin.";
            return View(model);
        }
    }
}