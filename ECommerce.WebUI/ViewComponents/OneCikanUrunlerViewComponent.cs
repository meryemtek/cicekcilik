using ECommerce.Application.DTOs.Urun;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace ECommerce.WebUI.ViewComponents
{
    public class OneCikanUrunlerViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OneCikanUrunlerViewComponent> _logger;

        public OneCikanUrunlerViewComponent(IHttpClientFactory httpClientFactory, ILogger<OneCikanUrunlerViewComponent> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var urunler = new List<UrunListeDto>();
            try
            {
                var response = await client.GetAsync("onecikanlar/urunler");
                if (response.IsSuccessStatusCode)
                {
                    urunler = await response.Content.ReadFromJsonAsync<List<UrunListeDto>>() ?? new List<UrunListeDto>();

                // API'nin base adresini alıyoruz (Örn: https://localhost:7001)
                var baseUrl = client.BaseAddress?.GetLeftPart(UriPartial.Authority) ?? "https://localhost:7001";

                // Görsel URL'lerini tam adrese dönüştürüyoruz
                    foreach (var urun in urunler)
                    {
                        if (!string.IsNullOrWhiteSpace(urun.GorselUrl) && !urun.GorselUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                        {
                            urun.GorselUrl = $"{baseUrl.TrimEnd('/')}/{urun.GorselUrl.TrimStart('/')}";
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("Öne çıkan ürünler API'den yüklenemedi: {StatusCode}", response.StatusCode);
                    ViewData["ApiYuklenemedi"] = true;
                }
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
            {
                _logger.LogWarning(ex, "Öne çıkan ürünler API'den yüklenemedi.");
                ViewData["ApiYuklenemedi"] = true;
            }

            return View(urunler);
        }
    }
}
