using ECommerce.Application.DTOs.Kategori;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace ECommerce.WebUI.ViewComponents
{
    public class HeaderKategoriMenuViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HeaderKategoriMenuViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
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
            catch
            {
                return View(new List<KategoriListeDto>());
            }
        }
    }
}
