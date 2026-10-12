using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using ECommerce.Application.DTOs.Menu; 

namespace ECommerce.WebUI.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MenuViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(string konum = "Header")
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            try
            {
                var menuler = await client.GetFromJsonAsync<List<MenuItemListeDto>>($"admin/menuler/konum/{konum}");

                // Konuma göre dikey (Footer) veya yatay (Header) view döndürüyoruz
                string viewName = konum.Equals("Footer", StringComparison.OrdinalIgnoreCase) ? "FooterMenu" : "Default";

                return View(viewName, menuler ?? new List<MenuItemListeDto>());
            }
            catch
            {
                return View(konum.Equals("Footer", StringComparison.OrdinalIgnoreCase) ? "FooterMenu" : "Default", new List<MenuItemListeDto>());
            }
        }
    }
}