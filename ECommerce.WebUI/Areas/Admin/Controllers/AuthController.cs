using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ECommerce.WebUI.Models;
using System.Collections.Generic;

namespace ECommerce.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
public class AuthController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        return View(new LoginViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        // 1. WEBUI DEDEKTİFİ: Ekrana yazılan veriler buraya kadar gelebiliyor mu?
        if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Sifre))
        {
            ModelState.AddModelError(string.Empty, "WEBUI DEDEKTİFİ: E-posta veya şifre HTML formdan arka plana ulaşamadı! Form gönderimi boş.");
            return View(model);
        }

        if (!ModelState.IsValid)
            return View(model);

        var client = _httpClientFactory.CreateClient("ApiClient");

        // 2. KESİN ÇÖZÜM: API'nin JSON formatını yadırgamaması için veriyi en standart Sözlük (Dictionary) yapısıyla gönderiyoruz.
        var veriPaketi = new Dictionary<string, string>
        {
            { "email", model.Email },
            { "sifre", model.Sifre }
        };

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(veriPaketi),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync("auth/login", jsonContent);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<TokenResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (tokenData != null && !string.IsNullOrEmpty(tokenData.Token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(tokenData.Token);
                var claims = new List<Claim>(jwtToken.Claims);
                claims.Add(new Claim("JWToken", tokenData.Token));

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { IsPersistent = model.BeniHatirla };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
        }
        else
        {
            // API'den gelen mesajı direkt ekrana yansıtıyoruz
            var errorDetail = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Giriş başarısız. Detay: {errorDetail}");
            return View(model);
        }

        ModelState.AddModelError(string.Empty, "Beklenmeyen bir hata oluştu.");
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Auth", new { area = "Admin" });
    }
}