using Microsoft.AspNetCore.Mvc;
using ECommerce.Application.DTOs.Siparis;
using ECommerce.Domain.Enums;
using System.Net.Http.Headers;
using System.Text.Json;

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

            var yeniSiparisler = new List<SiparisListeDto>();
            try
            {
                var response = await client.GetAsync("admin/siparisler");
                if (response.IsSuccessStatusCode)
                {
                    var siparisler = await response.Content.ReadFromJsonAsync<List<SiparisListeDto>>(
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    yeniSiparisler = siparisler?
                        .Where(x => x.Durum == SiparisDurumu.Beklemede)
                        .OrderByDescending(x => x.SiparisTarihi)
                        .ToList() ?? new List<SiparisListeDto>();
                }
            }
            catch { }

            ViewBag.OkunmamisMesajSayisi = okunmamisMesajSayisi;
            ViewBag.YeniSiparisler = yeniSiparisler;
            ViewBag.ToplamBildirim = okunmamisMesajSayisi + yeniSiparisler.Count;

            if (viewName == "Badge")
            {
                return View("Badge");
            }

            return View("Default");
        }
    }
}
