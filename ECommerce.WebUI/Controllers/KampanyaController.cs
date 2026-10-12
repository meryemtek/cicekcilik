using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using ECommerce.Application.DTOs.Kampanya;

namespace ECommerce.WebUI.Controllers
{
    public class KampanyaController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public KampanyaController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("kampanyalar")]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("kampanya/aktif-liste");

            List<KampanyaUrunDto> model = new();
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                model = JsonSerializer.Deserialize<List<KampanyaUrunDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

                if (model.Any())
                {
                    var baseUrl = client.BaseAddress?.GetLeftPart(UriPartial.Authority) ?? "https://localhost:7001";
                    foreach (var item in model)
                    {
                        if (!string.IsNullOrWhiteSpace(item.GorselUrl) && !item.GorselUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                        {
                            item.GorselUrl = $"{baseUrl.TrimEnd('/')}/{item.GorselUrl.TrimStart('/')}";
                        }
                    }
                }
            }

            return View(model);
        }
    }
}