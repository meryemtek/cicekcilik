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
        private readonly ILogger<DuyuruBandViewComponent> _logger;

        public DuyuruBandViewComponent(IHttpClientFactory httpClientFactory, ILogger<DuyuruBandViewComponent> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
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
                _logger.LogWarning(ex, "Duyurular API'den yüklenemedi.");
                ViewData["ApiYuklenemedi"] = true;
            }

            return View(duyurular ?? new List<DuyuruDto>());
        }
    }
}
