using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.DTOs.Siparis;
using ECommerce.Application.DTOs.Adres;
using ECommerce.Application.DTOs.Favori;
using ECommerce.WebUI.Models;
using System.Net;

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
                return User.IsInRole("Admin")
                    ? RedirectToAction("Index", "Urun", new { area = "Admin" })
                    : RedirectToAction("Index");
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
                    var yoneticiMi = await OturumAcAsync(tokenCevap.Token, Request.Form.ContainsKey("BeniHatirla"));

                    TempData["BasariliMesaj"] = "Başarıyla giriş yaptınız.";
                    return yoneticiMi
                        ? RedirectToAction("Index", "Urun", new { area = "Admin" })
                        : RedirectToAction("Index");
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
        public async Task<IActionResult> Cikis()
        {
            Response.Cookies.Delete("JWToken");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["BasariliMesaj"] = "Oturum kapatıldı.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!Request.Cookies.ContainsKey("JWToken"))
            {
                return RedirectToAction(nameof(Giris));
            }

            var client = CreateClientWithToken();
            if (!await ProfilYukleAsync(client)) return RedirectToAction(nameof(Giris));

            var model = new HesapOzetViewModel { AdSoyad = ViewData["HesapAdSoyad"]?.ToString() ?? string.Empty };
            var siparisYanit = await client.GetAsync("siparis/benim-siparislerim");
            if (siparisYanit.IsSuccessStatusCode)
            {
                var siparisler = await siparisYanit.Content.ReadFromJsonAsync<List<SiparisListeDto>>() ?? new();
                model.SiparisSayisi = siparisler.Count;
                model.SonSiparis = siparisler.OrderByDescending(s => s.SiparisTarihi).FirstOrDefault();
            }
            var favoriYanit = await client.GetAsync("favori/00000000-0000-0000-0000-000000000000");
            if (favoriYanit.IsSuccessStatusCode)
                model.FavoriSayisi = (await favoriYanit.Content.ReadFromJsonAsync<List<FavoriListeDto>>())?.Count ?? 0;
            var adresYanit = await client.GetAsync("hesap/adresler");
            if (adresYanit.IsSuccessStatusCode)
                model.AdresSayisi = (await adresYanit.Content.ReadFromJsonAsync<List<KayitliAdresDto>>())?.Count ?? 0;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Siparislerim()
        {
            if (!Request.Cookies.ContainsKey("JWToken"))
            {
                return RedirectToAction(nameof(Giris));
            }

            var client = CreateClientWithToken();
            if (!await ProfilYukleAsync(client)) return RedirectToAction(nameof(Giris));
            var response = await client.GetAsync("siparis/benim-siparislerim");

            if (response.StatusCode == HttpStatusCode.Unauthorized) return OturumGecersiz();

            var siparisler = new List<SiparisListeDto>();
            if (response.IsSuccessStatusCode)
            {
                siparisler = await response.Content.ReadFromJsonAsync<List<SiparisListeDto>>() ?? new List<SiparisListeDto>();
            }

            return View(siparisler);
        }

        [HttpGet]
        public async Task<IActionResult> SiparisDetay(Guid id)
        {
            if (!Request.Cookies.ContainsKey("JWToken")) return RedirectToAction(nameof(Giris));
            var client = CreateClientWithToken();
            if (!await ProfilYukleAsync(client)) return RedirectToAction(nameof(Giris));
            var response = await client.GetAsync($"siparis/benim-siparislerim/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound) return NotFound();
            if (response.StatusCode == HttpStatusCode.Unauthorized) return OturumGecersiz();
            if (!response.IsSuccessStatusCode) return StatusCode(502);
            var siparis = await response.Content.ReadFromJsonAsync<SiparisDetayDto>();
            return siparis == null ? NotFound() : View(siparis);
        }

        [HttpGet]
        public async Task<IActionResult> Adreslerim(Guid? duzenle)
        {
            if (!Request.Cookies.ContainsKey("JWToken")) return RedirectToAction(nameof(Giris));
            var client = CreateClientWithToken();
            if (!await ProfilYukleAsync(client)) return RedirectToAction(nameof(Giris));
            var response = await client.GetAsync("hesap/adresler");
            if (!response.IsSuccessStatusCode)
            {
                TempData["HataMesaj"] = "Adresler yüklenemedi. Lütfen tekrar deneyin.";
                return View(new KayitliAdresSayfaViewModel());
            }
            var model = new KayitliAdresSayfaViewModel
            {
                Adresler = await response.Content.ReadFromJsonAsync<List<KayitliAdresDto>>() ?? new()
            };
            if (duzenle.HasValue)
            {
                var adres = model.Adresler.FirstOrDefault(a => a.Id == duzenle.Value);
                if (adres == null) return NotFound();
                model.DuzenlenenId = adres.Id;
                model.Form = new KayitliAdresKaydetDto
                {
                    Baslik = adres.Baslik, AliciAdSoyad = adres.AliciAdSoyad,
                    Telefon = adres.Telefon, Il = adres.Il, Ilce = adres.Ilce,
                    AcikAdres = adres.AcikAdres, VarsayilanMi = adres.VarsayilanMi
                };
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdresKaydet(Guid? id, KayitliAdresKaydetDto form)
        {
            if (!Request.Cookies.ContainsKey("JWToken")) return RedirectToAction(nameof(Giris));
            if (!ModelState.IsValid)
            {
                var model = new KayitliAdresSayfaViewModel { Form = form, DuzenlenenId = id };
                var clientForList = CreateClientWithToken();
                if (!await ProfilYukleAsync(clientForList)) return RedirectToAction(nameof(Giris));
                var listResponse = await clientForList.GetAsync("hesap/adresler");
                if (listResponse.IsSuccessStatusCode)
                    model.Adresler = await listResponse.Content.ReadFromJsonAsync<List<KayitliAdresDto>>() ?? new();
                return View("Adreslerim", model);
            }
            var client = CreateClientWithToken();
            var response = id.HasValue
                ? await client.PutAsJsonAsync($"hesap/adresler/{id.Value}", form)
                : await client.PostAsJsonAsync("hesap/adresler", form);
            if (response.StatusCode == HttpStatusCode.Unauthorized) return OturumGecersiz();
            TempData[response.IsSuccessStatusCode ? "BasariliMesaj" : "HataMesaj"] = response.IsSuccessStatusCode
                ? "Adresiniz kaydedildi." : "Adres kaydedilemedi. Bilgileri kontrol edip tekrar deneyin.";
            return RedirectToAction(nameof(Adreslerim));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdresSil(Guid id)
        {
            if (!Request.Cookies.ContainsKey("JWToken")) return RedirectToAction(nameof(Giris));
            var response = await CreateClientWithToken().DeleteAsync($"hesap/adresler/{id}");
            if (response.StatusCode == HttpStatusCode.Unauthorized) return OturumGecersiz();
            TempData[response.IsSuccessStatusCode ? "BasariliMesaj" : "HataMesaj"] = response.IsSuccessStatusCode
                ? "Adres silindi." : "Adres silinemedi.";
            return RedirectToAction(nameof(Adreslerim));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VarsayilanAdresYap(Guid id)
        {
            if (!Request.Cookies.ContainsKey("JWToken")) return RedirectToAction(nameof(Giris));
            var response = await CreateClientWithToken().PostAsync($"hesap/adresler/{id}/varsayilan", null);
            if (response.StatusCode == HttpStatusCode.Unauthorized) return OturumGecersiz();
            TempData[response.IsSuccessStatusCode ? "BasariliMesaj" : "HataMesaj"] = response.IsSuccessStatusCode
                ? "Varsayılan adres güncellendi." : "Varsayılan adres değiştirilemedi.";
            return RedirectToAction(nameof(Adreslerim));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SifreDegistir(SifreDegistirDto dto)
        {
            if (!Request.Cookies.ContainsKey("JWToken")) return RedirectToAction(nameof(Giris));
            if (!ModelState.IsValid)
            {
                TempData["HataMesaj"] = "Yeni şifre en az 6 karakter olmalı ve tekrarı aynı olmalı.";
                return RedirectToAction(nameof(Index));
            }
            var response = await CreateClientWithToken().PostAsJsonAsync("auth/sifre-degistir", dto);
            if (response.StatusCode == HttpStatusCode.Unauthorized) return OturumGecersiz();
            TempData[response.IsSuccessStatusCode ? "BasariliMesaj" : "HataMesaj"] = response.IsSuccessStatusCode
                ? "Şifreniz güncellendi." : "Şifre güncellenemedi. Mevcut şifrenizi kontrol edin.";
            return RedirectToAction(nameof(Index));
        }

        // Müşteri ve yönetici aynı girişi kullanır; cookie kimliği JWT rolleriyle kurulur,
        // böylece [Authorize(Roles = "Admin")] korumalı admin ekranları bu oturumla açılır.
        private async Task<bool> OturumAcAsync(string token, bool beniHatirla)
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var yoneticiMi = jwt.Claims.Any(c => c.Type == "role" && c.Value == "Admin");
            var claims = jwt.Claims.Select(c => new Claim(c.Type switch
            {
                "role" => ClaimTypes.Role,
                "nameid" => ClaimTypes.NameIdentifier,
                "email" => ClaimTypes.Email,
                "unique_name" => ClaimTypes.Name,
                _ => c.Type
            }, c.Value)).ToList();
            claims.Add(new Claim("JWToken", token));

            var kimlik = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(kimlik),
                new AuthenticationProperties { IsPersistent = beniHatirla });

            return yoneticiMi;
        }

        private async Task<bool> ProfilYukleAsync(HttpClient client)
        {
            var response = await client.GetAsync("auth/benim-profilim");
            if (!response.IsSuccessStatusCode)
            {
                Response.Cookies.Delete("JWToken");
                return false;
            }
            var profil = await response.Content.ReadFromJsonAsync<HesapProfilDto>();
            if (profil == null)
            {
                Response.Cookies.Delete("JWToken");
                return false;
            }
            ViewData["HesapAdSoyad"] = profil.AdSoyad;
            return true;
        }

        private IActionResult OturumGecersiz()
        {
            Response.Cookies.Delete("JWToken");
            return RedirectToAction(nameof(Giris));
        }

        [HttpPost]
        public async Task<IActionResult> SifremiUnuttum([FromBody] SifremiUnuttumDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.Email))
            {
                return Json(new { basarili = false, mesaj = "Lütfen e-posta adresinizi giriniz." });
            }

            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.PostAsJsonAsync("auth/sifremi-unuttum", dto);

            if (response.IsSuccessStatusCode)
            {
                return Json(new { basarili = true, mesaj = "Şifre sıfırlama bağlantısı e-posta adresinize gönderildi. Lütfen gelen kutunuzu kontrol edin." });
            }

            return Json(new { basarili = false, mesaj = "Talep iletilirken bir sorun oluştu. Lütfen e-posta adresinizi kontrol ediniz." });
        }

        [HttpGet]
        public IActionResult SifreSifirla(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                TempData["HataMesaj"] = "Geçersiz veya süresi dolmuş şifre sıfırlama bağlantısı.";
                return RedirectToAction(nameof(Giris));
            }

            var model = new SifreSifirlaDto
            {
                Email = email,
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SifreSifirla(SifreSifirlaDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.PostAsJsonAsync("auth/sifre-sifirla", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Şifreniz başarıyla sıfırlandı. Yeni şifrenizle giriş yapabilirsiniz.";
                return RedirectToAction(nameof(Giris));
            }

            ModelState.AddModelError(string.Empty, "Şifre sıfırlanamadı. Bağlantının süresi dolmuş olabilir. Lütfen tekrar deneyiniz.");
            return View(dto);
        }
    }
}
