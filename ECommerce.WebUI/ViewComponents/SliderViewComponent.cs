using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ECommerce.Application.DTOs.Slider;

namespace ECommerce.WebUI.ViewComponents
{
    public class SliderViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SliderViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            // WebAPI tarafında aktif slider'ları çekeceğimiz endpoint (Daha önce yazdıysak veya API'ye ekleyeceğiz)
            List<SliderListeDto>? sliderlar = null;
            try
            {
                sliderlar = await client.GetFromJsonAsync<List<SliderListeDto>>("slider/aktif");
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
            {
                // API geçici olarak yanıt vermese de anasayfa gösterilir.
            }

            return View(sliderlar ?? new List<SliderListeDto>());
        }
    }
}
