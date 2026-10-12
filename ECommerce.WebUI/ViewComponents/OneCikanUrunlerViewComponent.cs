using ECommerce.Application.DTOs.Urun;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace ECommerce.WebUI.ViewComponents
{
    public class OneCikanUrunlerViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OneCikanUrunlerViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("onecikanlar/urunler");

            var urunler = new List<UrunListeDto>();
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

            return View(urunler);
        }
    }
}