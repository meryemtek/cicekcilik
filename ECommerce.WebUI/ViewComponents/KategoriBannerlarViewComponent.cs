using ECommerce.Application.DTOs.KategoriBanner;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace ECommerce.WebUI.ViewComponents
{
    public class KategoriBannerlarViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public KategoriBannerlarViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var liste = new List<KategoriBannerDto>();
            try
            {
                var response = await client.GetAsync("kategori-bannerlar");
                if (response.IsSuccessStatusCode)
                {
                    liste = await response.Content.ReadFromJsonAsync<List<KategoriBannerDto>>() ?? new List<KategoriBannerDto>();
                    var baseUrl = client.BaseAddress?.GetLeftPart(UriPartial.Authority) ?? "https://localhost:7001";

                    foreach (var item in liste)
                    {
                        if (!string.IsNullOrWhiteSpace(item.GorselUrl) && !item.GorselUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                        {
                            item.GorselUrl = $"{baseUrl.TrimEnd('/')}/{item.GorselUrl.TrimStart('/')}";
                        }
                    }
                }
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
            {
                // API geçici olarak yanıt vermese de anasayfa gösterilir.
            }

            return View(liste.Where(x => x.AktifMi).ToList());
        }
    }
}
