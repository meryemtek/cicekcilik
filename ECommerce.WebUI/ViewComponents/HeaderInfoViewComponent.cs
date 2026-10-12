using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using ECommerce.Application.DTOs.SiteAyari;

namespace ECommerce.WebUI.ViewComponents
{
    public class HeaderInfoViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HeaderInfoViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            // API controller'ın rotasıyla tam eşleşen adres
            var ayarlar = await client.GetFromJsonAsync<SiteAyariDto>("siteayarlari/site-ayarlari");

            return View(ayarlar ?? new SiteAyariDto());
        }
    }
}