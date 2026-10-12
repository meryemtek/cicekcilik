using ECommerce.Application.DTOs.Kampanya;
using ECommerce.Application.DTOs.Kategori;
using ECommerce.Application.DTOs.Urun;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECommerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class KampanyaYonetimiController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public KampanyaYonetimiController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateClientWithToken()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var token = User.FindFirst("JWToken")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        // LİSTELEME
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = CreateClientWithToken();
            var response = await client.GetAsync("kampanya/liste");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var list = JsonSerializer.Deserialize<List<KampanyaUrunDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (list != null)
                {
                    // Ürün controller'daki gibi resim yollarını tam URL'e dönüştürüyoruz
                    var baseUrl = client.BaseAddress?.GetLeftPart(UriPartial.Authority) ?? "https://localhost:7001";

                    foreach (var item in list)
                    {
                        if (!string.IsNullOrWhiteSpace(item.GorselUrl) && !item.GorselUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                        {
                            item.GorselUrl = $"{baseUrl.TrimEnd('/')}/{item.GorselUrl.TrimStart('/')}";
                        }
                    }
                }

                return View(list ?? new List<KampanyaUrunDto>());
            }

            return View(new List<KampanyaUrunDto>());
        }

        // EKLEME SAYFASI (GET)
        [HttpGet]
        public async Task<IActionResult> Ekle()
        {
            var client = CreateClientWithToken();
            var urunResponse = await client.GetAsync("admin/urunler");

            // DÜZELTME: API'deki doğru rota olan "kategoriler" kullanıldı
            var kategoriResponse = await client.GetAsync("kategoriler");

            List<SelectListItem> urunSecenekleri = new();
            if (urunResponse.IsSuccessStatusCode)
            {
                var content = await urunResponse.Content.ReadAsStringAsync();
                var urunler = JsonSerializer.Deserialize<List<UrunListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (urunler != null)
                {
                    urunSecenekleri = urunler.Select(u => new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = $"{u.Ad} (Kod: {u.UrunKodu ?? "-"}) - {u.Fiyat:N2} ₺"
                    }).ToList();
                }
            }

            List<SelectListItem> kategoriSecenekleri = new();
            if (kategoriResponse.IsSuccessStatusCode)
            {
                var content = await kategoriResponse.Content.ReadAsStringAsync();
                var kategoriler = JsonSerializer.Deserialize<List<KategoriListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (kategoriler != null)
                {
                    kategoriSecenekleri = kategoriler.Select(k => new SelectListItem
                    {
                        Value = k.Id.ToString(),
                        Text = k.Ad
                    }).ToList();
                }
            }

            ViewBag.Urunler = urunSecenekleri;
            ViewBag.Kategoriler = kategoriSecenekleri;
            return View();
        }

        // EKLEME İŞLEMİ (POST)
        [HttpPost]
        public async Task<IActionResult> Ekle(KampanyaUrunEkleDto dto)
        {
            var client = CreateClientWithToken();
            var response = await client.PostAsJsonAsync("kampanya/ekle", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Kampanya başarıyla oluşturuldu ve eklendi.";
                return RedirectToAction("Index");
            }

            TempData["HataMesaji"] = "Kampanya eklenirken bir hata oluştu.";
            return RedirectToAction("Ekle");
        }

        // GÜNCELLEME SAYFASI (GET)
        [HttpGet]
        public async Task<IActionResult> Guncelle(Guid id)
        {
            var client = CreateClientWithToken();
            var kampanyaResponse = await client.GetAsync("kampanya/liste");

            // DÜZELTME: API'deki doğru rota olan "kategoriler" kullanıldı
            var kategoriResponse = await client.GetAsync("kategoriler");
            var urunResponse = await client.GetAsync("admin/urunler");

            List<SelectListItem> urunSecenekleri = new();
            if (urunResponse.IsSuccessStatusCode)
            {
                var content = await urunResponse.Content.ReadAsStringAsync();
                var urunler = JsonSerializer.Deserialize<List<UrunListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (urunler != null)
                {
                    urunSecenekleri = urunler.Select(u => new SelectListItem { Value = u.Id.ToString(), Text = $"{u.Ad} ({u.UrunKodu ?? "-"}) - {u.Fiyat:N2} ₺" }).ToList();
                }
            }

            List<SelectListItem> kategoriSecenekleri = new();
            if (kategoriResponse.IsSuccessStatusCode)
            {
                var content = await kategoriResponse.Content.ReadAsStringAsync();
                var kategoriler = JsonSerializer.Deserialize<List<KategoriListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (kategoriler != null)
                {
                    kategoriSecenekleri = kategoriler.Select(k => new SelectListItem { Value = k.Id.ToString(), Text = k.Ad }).ToList();
                }
            }

            ViewBag.Urunler = urunSecenekleri;
            ViewBag.Kategoriler = kategoriSecenekleri;

            if (kampanyaResponse.IsSuccessStatusCode)
            {
                var content = await kampanyaResponse.Content.ReadAsStringAsync();
                var list = JsonSerializer.Deserialize<List<KampanyaUrunDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                var kampanya = list?.FirstOrDefault(x => x.Id == id);

                if (kampanya != null)
                {
                    ViewBag.UrunAdi = kampanya.UrunAdi;
                    ViewBag.Gorsel = kampanya.GorselUrl;

                    var guncelleDto = new KampanyaUrunGuncelleDto
                    {
                        Id = kampanya.Id,
                        KampanyaAdi = kampanya.KampanyaAdi,
                        KampanyaAciklamasi = kampanya.KampanyaAciklamasi,
                        IndirimTipi = kampanya.IndirimTipi,
                        IndirimDegeri = kampanya.IndirimDegeri,
                        KampanyaVaryasyonu = kampanya.KampanyaVaryasyonu,
                        TetikleyiciKategoriId = kampanya.TetikleyiciKategoriId,
                        HedefKategoriId = kampanya.HedefKategoriId,
                        KuponlarlaBirlesebilirMi = kampanya.KuponlarlaBirlesebilirMi,
                        Sira = kampanya.Sira,
                        AktifMi = kampanya.AktifMi,
                        BaslangicTarihi = kampanya.BaslangicTarihi,
                        BitisTarihi = kampanya.BitisTarihi
                    };
                    return View(guncelleDto);
                }
            }

            TempData["HataMesaji"] = "Güncellenecek kampanya bulunamadı.";
            return RedirectToAction("Index");
        }

        // GÜNCELLEME İŞLEMİ (POST)
        [HttpPost]
        public async Task<IActionResult> Guncelle(KampanyaUrunGuncelleDto dto)
        {
            var client = CreateClientWithToken();
            var response = await client.PutAsJsonAsync("kampanya/guncelle", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Kampanya bilgileri başarıyla güncellendi.";
                return RedirectToAction("Index");
            }

            TempData["HataMesaji"] = "Güncelleme sırasında bir hata oluştu.";
            return RedirectToAction("Index");
        }

        // SİLME İŞLEMİ (POST)
        [HttpPost]
        public async Task<IActionResult> Sil(Guid id)
        {
            var client = CreateClientWithToken();
            var response = await client.DeleteAsync($"kampanya/sil/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Kampanya sistemden kaldırıldı.";
            }
            else
            {
                TempData["HataMesaji"] = "Silme işlemi başarısız oldu.";
            }

            return RedirectToAction("Index");
        }
    }
}