using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ECommerce.Application.DTOs.Urun;

namespace ECommerce.WebUI.ViewComponents
{
    public class BenzerUrunlerViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BenzerUrunlerViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private string GetFullImageUrl(HttpClient client, string gorselUrl)
        {
            if (string.IsNullOrWhiteSpace(gorselUrl)) return string.Empty;
            if (gorselUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return gorselUrl;

            var baseUrl = client.BaseAddress?.GetLeftPart(UriPartial.Authority) ?? "https://localhost:7001";
            return $"{baseUrl.TrimEnd('/')}/{gorselUrl.TrimStart('/')}".Replace("\\", "/");
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid kategoriId, Guid mevcutUrunId)
        {
            List<UrunListeDto> benzerUrunler = new List<UrunListeDto>();

            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"urunler/kategori/{kategoriId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var urunler = JsonSerializer.Deserialize<List<UrunListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (urunler != null)
                {
                    benzerUrunler = urunler.Where(x => x.Id != mevcutUrunId).Take(4).ToList();

                    // Görsel yollarını tam URL formatına dönüştürüyoruz
                    foreach (var urun in benzerUrunler)
                    {
                        urun.GorselUrl = GetFullImageUrl(client, urun.GorselUrl);
                    }
                }
            }

            return View(benzerUrunler);
        }
    }
}