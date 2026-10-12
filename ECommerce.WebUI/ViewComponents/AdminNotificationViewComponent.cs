using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace ECommerce.WebUI.ViewComponents
{
    public class AdminNotificationViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminNotificationViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(string viewName = "Default")
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var token = HttpContext.User?.FindFirst("JWToken")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // EĞER ÖDEME BİLDİRİMLERİ İÇİN ÇAĞRILDIYSA
            if (viewName == "OdemeBadge")
            {
                int okunmamisOdemeSayisi = 0;
                try
                {
                    var response = await client.GetAsync("admin/odeme-bildirimleri/okunmamis-sayi");
                    if (response.IsSuccessStatusCode)
                    {
                        int.TryParse(await response.Content.ReadAsStringAsync(), out okunmamisOdemeSayisi);
                    }
                }
                catch { }

                ViewBag.OkunmamisOdemeSayisi = okunmamisOdemeSayisi;
                return View("OdemeBadge");
            }

            // Standart İletişim Mesajları / Çan Mantığı
            int okunmamisMesajSayisi = 0;
            try
            {
                var response = await client.GetAsync("admin/iletisim-mesajlari/okunmamis-sayi");
                if (response.IsSuccessStatusCode)
                {
                    int.TryParse(await response.Content.ReadAsStringAsync(), out okunmamisMesajSayisi);
                }
            }
            catch { }

            ViewBag.OkunmamisMesajSayisi = okunmamisMesajSayisi;
            ViewBag.ToplamBildirim = okunmamisMesajSayisi;

            if (viewName == "Badge")
            {
                return View("Badge");
            }

            return View("Default");
        }
    }
}