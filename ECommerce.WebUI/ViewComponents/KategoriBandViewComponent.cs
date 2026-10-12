using ECommerce.Application.DTOs.AnasayfaKategori;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace ECommerce.WebUI.ViewComponents
{
    public class KategoriBandViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public KategoriBandViewComponent(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            var kategoriler =
                await client.GetFromJsonAsync<List<AnasayfaKategoriDto>>(
                    "anasayfa-kategoriler/aktif");

            return View(
                kategoriler ?? new List<AnasayfaKategoriDto>());
        }
    }
}