using ECommerce.Application.DTOs.Kampanya;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECommerce.WebUI.ViewComponents
{
    public class KampanyalarViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public KampanyalarViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            // Sadece aktif ve süresi geçmemiş olanları listeleriz
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