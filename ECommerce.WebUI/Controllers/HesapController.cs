using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.DTOs.Siparis;

namespace ECommerce.WebUI.Controllers
{
    public class HesapController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HesapController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateClientWithToken()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var token = Request.Cookies["JWToken"];

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        [HttpGet]
        public IActionResult Giris()
        {
            if (Request.Cookies.ContainsKey("JWToken"))
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Giris(GirisDto dto)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.PostAsJsonAsync("auth/login", dto);

            if (response.IsSuccessStatusCode)
            {
                var tokenCevap = await response.Content.ReadFromJsonAsync<TokenDto>();
                if (tokenCevap != null && !string.IsNullOrEmpty(tokenCevap.Token))
                {
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(7),
                        HttpOnly = true,
                        IsEssential = true,
                        Secure = true
                    };
                    Response.Cookies.Append("JWToken", tokenCevap.Token, cookieOptions);

                    TempData["BasariliMesaj"] = "Başarıyla giriş yaptınız.";
                    return RedirectToAction("Index");
                }
            }

            ModelState.AddModelError(string.Empty, "E-posta veya şifre hatalı.");
            return View(dto);
        }

        [HttpGet]
        public IActionResult Kayit()
        {
            if (Request.Cookies.ContainsKey("JWToken"))
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Kayit(KayitDto dto)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.PostAsJsonAsync("auth/kayit-ol", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Kayıt işlemi başarılı. Lütfen giriş yapınız.";
                return RedirectToAction(nameof(Giris));
            }

            ModelState.AddModelError(string.Empty, "Kayıt oluşturulurken bir hata oluştu. E-posta adresi kullanımda olabilir.");
            return View(dto);
        }

        [HttpGet]
        public IActionResult Cikis()
        {
            Response.Cookies.Delete("JWToken");
            TempData["BasariliMesaj"] = "Oturum kapatıldı.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (!Request.Cookies.ContainsKey("JWToken"))
            {
                return RedirectToAction(nameof(Giris));
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Siparislerim()
        {
            if (!Request.Cookies.ContainsKey("JWToken"))
            {
                return RedirectToAction(nameof(Giris));
            }

            var client = CreateClientWithToken();
            var response = await client.GetAsync("siparis/benim-siparislerim");

            var siparisler = new List<SiparisListeDto>();
            if (response.IsSuccessStatusCode)
            {
                siparisler = await response.Content.ReadFromJsonAsync<List<SiparisListeDto>>() ?? new List<SiparisListeDto>();
            }

            return View(siparisler);
        }
    }
}