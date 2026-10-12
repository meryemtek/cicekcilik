using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using ECommerce.Application.DTOs.Favori;

namespace ECommerce.WebUI.Controllers
{
    public class FavorilerController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string VisitorCookieName = "VisitorId";

        public FavorilerController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private string GetOrSetVisitorId()
        {
            if (Request.Cookies.TryGetValue(VisitorCookieName, out var visitorId) && !string.IsNullOrEmpty(visitorId))
            {
                return visitorId;
            }

            visitorId = Guid.NewGuid().ToString();
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                HttpOnly = true,
                IsEssential = true
            };
            Response.Cookies.Append(VisitorCookieName, visitorId, cookieOptions);
            return visitorId;
        }

        private HttpClient CreateClientWithToken()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var token = User.FindFirst("JWToken")?.Value ?? Request.Cookies["JWToken"];

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        // UrunController'daki gibi tam görsel yolu oluşturan metodumuz
        private string GetFullImageUrl(string gorselUrl)
        {
            if (string.IsNullOrWhiteSpace(gorselUrl)) return "/images/no-image.jpg";
            if (gorselUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return gorselUrl;

            var client = _httpClientFactory.CreateClient("ApiClient");
            var baseUrl = client.BaseAddress?.GetLeftPart(UriPartial.Authority) ?? "https://localhost:7001";

            return $"{baseUrl.TrimEnd('/')}/{gorselUrl.TrimStart('/')}".Replace("\\", "/");
        }

        public async Task<IActionResult> Index()
        {
            var visitorId = GetOrSetVisitorId();
            var client = CreateClientWithToken();

            var response = await client.GetAsync($"favori/{visitorId}");
            var favoriler = new List<FavoriListeDto>();

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                favoriler = JsonSerializer.Deserialize<List<FavoriListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

                // Gelen her favori ürünün görselini tam yola dönüştürüyoruz
                foreach (var item in favoriler)
                {
                    item.GorselUrl = GetFullImageUrl(item.GorselUrl);
                }
            }

            return View(favoriler);
        }

        [HttpGet]
        public async Task<IActionResult> GetFavoriIdList()
        {
            var visitorId = GetOrSetVisitorId();
            var client = CreateClientWithToken();

            var response = await client.GetAsync($"favori/{visitorId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var favoriler = JsonSerializer.Deserialize<List<FavoriListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                var ids = favoriler.Select(f => f.UrunId).ToList();
                return Ok(ids);
            }

            return Ok(new List<Guid>());
        }

        [HttpPost]
        public async Task<IActionResult> Toggle([FromBody] FavoriToggleModel model)
        {
            if (model == null || model.UrunId == Guid.Empty)
            {
                return BadRequest(new { mesaj = "Geçersiz ürün." });
            }

            var visitorId = GetOrSetVisitorId();
            var client = CreateClientWithToken();

            var dto = new FavoriEkleDto
            {
                KullaniciId = visitorId,
                UrunId = model.UrunId
            };

            var response = await client.PostAsJsonAsync("favori/toggle", dto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<FavoriCevapDto>();
                return Ok(result ?? new FavoriCevapDto { EklendiMi = false });
            }

            return BadRequest(new { mesaj = "İşlem başarısız." });
        }
    }

    public class FavoriToggleModel
    {
        public Guid UrunId { get; set; }
    }

    public class FavoriCevapDto
    {
        public bool EklendiMi { get; set; }
    }
}