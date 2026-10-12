using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ECommerce.Application.DTOs.SiteAyari;

namespace ECommerce.WebUI.ViewComponents
{
    public class HeaderMainViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HeaderMainViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            // API'den site ayarlarını (Logo dahil) çekiyoruz
            var ayarlar = await client.GetFromJsonAsync<SiteAyariDto>("siteayarlari/site-ayarlari");

            return View(ayarlar ?? new SiteAyariDto());
        }
    }
}