using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using ECommerce.Application.DTOs.Hakkimizda;

namespace ECommerce.WebUI.ViewComponents
{
    public class HakkimizdaViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HakkimizdaViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var model = new HakkimizdaAyariDto();

            try
            {
                var response = await client.GetAsync("hakkimizda");
                if (response.IsSuccessStatusCode)
                {
                    model = await response.Content.ReadFromJsonAsync<HakkimizdaAyariDto>() ?? new();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Hakkımızda verileri çekilemedi: " + ex.Message);
            }

            return View(model);
        }
    }
}