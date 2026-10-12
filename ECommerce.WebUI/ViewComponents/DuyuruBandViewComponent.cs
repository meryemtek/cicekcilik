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

        public DuyuruBandViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            // WebAPI'den aktif duyuruları çekiyoruz
            var duyurular = await client.GetFromJsonAsync<List<DuyuruDto>>("duyurular/aktif");

            return View(duyurular ?? new List<DuyuruDto>());
        }
    }
}