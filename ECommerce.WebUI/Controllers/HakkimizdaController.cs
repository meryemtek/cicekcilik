using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using ECommerce.Application.DTOs.Hakkimizda;

namespace ECommerce.WebUI.Controllers
{
    public class HakkimizdaController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HakkimizdaController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("hakkimizda")]
        public async Task<IActionResult> Index()
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
                System.Diagnostics.Debug.WriteLine("Ön yüz hakkımızda verisi alınamadı: " + ex.Message);
            }

            ViewData["Title"] = !string.IsNullOrEmpty(model.SeoTitle) ? model.SeoTitle : "Hakkımızda";
            ViewData["SeoDescription"] = model.SeoDescription;

            return View(model);
        }
    }
}