using ECommerce.Application.DTOs.Kategori;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace ECommerce.WebUI.ViewComponents
{
    public class HeaderKategoriMenuViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HeaderKategoriMenuViewComponent> _logger;

        public HeaderKategoriMenuViewComponent(IHttpClientFactory httpClientFactory, ILogger<HeaderKategoriMenuViewComponent> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            try
            {
                var kategoriler = await client.GetFromJsonAsync<List<KategoriListeDto>>("kategoriler");
                var aktifKategoriler = kategoriler?.Where(x => x.AktifMi).ToList() ?? new List<KategoriListeDto>();

                return View(aktifKategoriler);
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
            {
                _logger.LogWarning(ex, "Kategori menüsü API'den yüklenemedi.");
                ViewData["ApiYuklenemedi"] = true;
                return View(new List<KategoriListeDto>());
            }
        }
    }
}
