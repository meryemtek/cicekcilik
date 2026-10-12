using ECommerce.Application.DTOs.Blog;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace ECommerce.WebUI.ViewComponents
{
    public class BlogViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BlogViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            // ÇALIŞAN ÖRNEKTEKİ GİBİ: Başındaki "api/" ifadesini kaldırdık
            var response = await client.GetAsync("blog/aktif-liste");

            if (!response.IsSuccessStatusCode) return Content("");

            // Çalışan örnekteki gibi modern ve güvenli JSON okuma yöntemi
            var yazilar = await response.Content.ReadFromJsonAsync<List<BlogListeDto>>() ?? new List<BlogListeDto>();

            // API'nin base adresini alıp görsel yollarını tam URL'e çeviriyoruz (Tıpkı ürünlerdeki gibi)
            var baseUrl = client.BaseAddress?.GetLeftPart(UriPartial.Authority) ?? "https://localhost:7001";
            foreach (var yazi in yazilar)
            {
                if (!string.IsNullOrWhiteSpace(yazi.GorselUrl) && !yazi.GorselUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    yazi.GorselUrl = $"{baseUrl.TrimEnd('/')}/{yazi.GorselUrl.TrimStart('/')}";
                }
            }

            // Sadece aktif olanları listele ve en yeni 3 tanesini al
            var model = yazilar.Where(x => x.AktifMi)
                               .OrderByDescending(x => x.YayinlanmaTarihi)
                               .Take(3)
                               .ToList();

            return View(model);
        }
    }
}