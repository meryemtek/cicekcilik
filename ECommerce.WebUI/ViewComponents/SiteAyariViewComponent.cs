using ECommerce.Application.DTOs.SiteAyari;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ECommerce.WebUI.ViewComponents
{
    public class SiteAyariViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SiteAyariViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("siteayarlari/site-ayarlari");

            SiteAyariDto ayarlar = new SiteAyariDto();
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                ayarlar = JsonSerializer.Deserialize<SiteAyariDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new SiteAyariDto();
            }

            return View(ayarlar);
        }
    }
}