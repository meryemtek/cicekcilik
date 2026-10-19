using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ECommerce.Application.DTOs.SiteAyari;
using ECommerce.Application.Models;

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

            var sepetAdedi = 0;
            if (HttpContext.Request.Cookies.TryGetValue("LidyaSepetId", out var sepetId)
                && !string.IsNullOrWhiteSpace(sepetId))
            {
                try
                {
                    var sepet = await client.GetFromJsonAsync<Sepet>(
                        $"sepet/{Uri.EscapeDataString(sepetId)}");
                    sepetAdedi = sepet?.Kalemler.Sum(k => k.Miktar) ?? 0;
                }
                catch (HttpRequestException)
                {
                    // Sepet sayacı yüklenemese de başlık ve sayfa gösterilmeye devam etsin.
                }
            }

            ViewData["SepetAdedi"] = sepetAdedi;

            return View(ayarlar ?? new SiteAyariDto());
        }
    }
}
