using ECommerce.Application.DTOs.AnasayfaKategori;
using ECommerce.WebUI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace ECommerce.WebUI.ViewComponents
{
    public class KategoriBandViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<KategoriBandViewComponent> _logger;

        public KategoriBandViewComponent(IHttpClientFactory httpClientFactory, ILogger<KategoriBandViewComponent> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            try
            {
                var kategoriler = await client.GetFromJsonAsync<List<AnasayfaKategoriDto>>("anasayfa-kategoriler/aktif");
                var aktifKategoriler = kategoriler?.Where(x => x.AktifMi).OrderBy(x => x.SiraNo).ToList() ?? new List<AnasayfaKategoriDto>();

                foreach (var kategori in aktifKategoriler)
                {
                    kategori.GorselUrl = GorselAdresi.GoruntulenebilirYap(kategori.GorselUrl, client.BaseAddress);
                }

                return View(aktifKategoriler);
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
            {
                _logger.LogWarning(ex, "Anasayfa kategorileri API'den yüklenemedi.");
                ViewData["ApiYuklenemedi"] = true;
                return View(new List<AnasayfaKategoriDto>());
            }
        }
    }
}
