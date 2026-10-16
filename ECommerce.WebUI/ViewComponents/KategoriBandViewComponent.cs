using ECommerce.Application.DTOs.AnasayfaKategori;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace ECommerce.WebUI.ViewComponents
{
    public class KategoriBandViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public KategoriBandViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            try
            {
                var kategoriler = await client.GetFromJsonAsync<List<AnasayfaKategoriDto>>("anasayfa-kategoriler/aktif");
                var aktifKategoriler = kategoriler?.Where(x => x.AktifMi).OrderBy(x => x.SiraNo).ToList() ?? new List<AnasayfaKategoriDto>();

                return View(aktifKategoriler);
            }
            catch
            {
                return View(new List<AnasayfaKategoriDto>());
            }
        }
    }
}
