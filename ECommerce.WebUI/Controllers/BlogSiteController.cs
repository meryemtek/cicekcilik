using ECommerce.Application.DTOs.Blog;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebUI.Controllers
{
    public class BlogSiteController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BlogSiteController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [Route("blog")]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("blog/aktif-liste");

            if (!response.IsSuccessStatusCode)
            {
                return View(new List<BlogListeDto>());
            }

            var yazilar = await response.Content.ReadFromJsonAsync<List<BlogListeDto>>() ?? new List<BlogListeDto>();

            // Görsel URL'lerini tam adrese dönüştürme
            var baseUrl = client.BaseAddress?.GetLeftPart(UriPartial.Authority) ?? "https://localhost:7001";
            foreach (var yazi in yazilar)
            {
                if (!string.IsNullOrWhiteSpace(yazi.GorselUrl) && !yazi.GorselUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    yazi.GorselUrl = $"{baseUrl.TrimEnd('/')}/{yazi.GorselUrl.TrimStart('/')}";
                }
            }

            // Aktif olanları tarihe göre sıralayıp modele veriyoruz
            var aktifYazilar = yazilar.Where(x => x.AktifMi)
                                      .OrderByDescending(x => x.YayinlanmaTarihi)
                                      .ToList();

            return View(aktifYazilar);
        }

        // Ziyaretçinin göreceği SeoUrl tabanlı detay sayfası
        [Route("blog/{seoUrl}")]
        public async Task<IActionResult> Detay(string seoUrl)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            // API üzerindeki public detay uç noktasına istek atıyoruz
            var response = await client.GetAsync($"blog/detay/{seoUrl}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound(); // Yazı bulunamazsa 404 döndür
            }

            var yazi = await response.Content.ReadFromJsonAsync<BlogGuncelleDto>();

            if (yazi == null) return NotFound();

            // Görsel URL'sini tam adrese dönüştürme (Ürünlerde yaptığımız mantık)
            if (!string.IsNullOrWhiteSpace(yazi.GorselUrl) && !yazi.GorselUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                var baseUrl = client.BaseAddress?.GetLeftPart(UriPartial.Authority) ?? "https://localhost:7001";
                yazi.GorselUrl = $"{baseUrl.TrimEnd('/')}/{yazi.GorselUrl.TrimStart('/')}";
            }

            return View(yazi);
        }
    }
}
