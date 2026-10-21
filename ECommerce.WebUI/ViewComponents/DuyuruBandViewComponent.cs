using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerce.Application.DTOs.Duyuru;

namespace ECommerce.WebUI.ViewComponents
{
    public class DuyuruBandViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DuyuruBandViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            List<DuyuruDto>? duyurular = null;
            try
            {
                duyurular = await client.GetFromJsonAsync<List<DuyuruDto>>("duyurular/aktif");
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
            {
                // API erişilemiyorsa sayfanın geri kalanı gösterilmeye devam eder.
            }

            return View(duyurular ?? new List<DuyuruDto>());
        }
    }
}
