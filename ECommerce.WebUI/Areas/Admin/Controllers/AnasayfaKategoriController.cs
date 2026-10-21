using ECommerce.Application.DTOs.AnasayfaKategori;
using ECommerce.Application.DTOs.Kategori;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace ECommerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AnasayfaKategoriController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AnasayfaKategoriController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateClientWithToken()
        {
            var client =
                _httpClientFactory.CreateClient("ApiClient");

            var token = User.FindFirst("JWToken")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        token);
            }

            return client;
        }

        public async Task<IActionResult> Index()
        {
            var client = CreateClientWithToken();

            var response =
                await client.GetAsync("anasayfa-kategoriler");

            if (response.IsSuccessStatusCode)
            {
                var content =
                    await response.Content.ReadAsStringAsync();

                var liste =
                    JsonSerializer.Deserialize<
                        List<AnasayfaKategoriDto>>(
                        content,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                return View(
                    liste ?? new List<AnasayfaKategoriDto>());
            }

            TempData["HataMesaji"] =
                "Anasayfa kategorileri alınamadı.";

            return View(
                new List<AnasayfaKategoriDto>());
        }

        [HttpGet]
        public async Task<IActionResult> Ekle()
        {
            await KategorileriYukleAsync();

            return View(
                new AnasayfaKategoriEkleDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ekle(
            AnasayfaKategoriEkleDto model,
            IFormFile? gorselDosyasi)
        {
            if (gorselDosyasi != null)
            {
                try
                {
                    model.GorselUrl = await VitrinGorseliniKaydetAsync(gorselDosyasi);
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("gorselDosyasi", ex.Message);
                    await KategorileriYukleAsync();
                    return View(model);
                }
            }

            if (!ModelState.IsValid)
            {
                await KategorileriYukleAsync();
                return View(model);
            }

            var client = CreateClientWithToken();

            var response =
                await client.PostAsJsonAsync(
                    "anasayfa-kategoriler",
                    model);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] =
                    "Kategori anasayfaya başarıyla eklendi.";

                return RedirectToAction(nameof(Index));
            }

            var hata =
                await response.Content.ReadAsStringAsync();

            ModelState.AddModelError(
                string.Empty,
                hata);

            await KategorileriYukleAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Guncelle(Guid id)
        {
            var client = CreateClientWithToken();

            var response =
                await client.GetAsync(
                    $"anasayfa-kategoriler/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var content =
                await response.Content.ReadAsStringAsync();

            var item =
                JsonSerializer.Deserialize<
                    AnasayfaKategoriDto>(
                    content,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (item == null)
                return NotFound();

            var model = new AnasayfaKategoriGuncelleDto
            {
                Id = item.Id,
                KategoriId = item.KategoriId,
                SiraNo = item.SiraNo,
                AktifMi = item.AktifMi
            };
            ViewBag.GorselOnizlemeUrl = item.GorselUrl;
            ViewBag.VitrinGorselOverrideUrl = item.VitrinGorselUrl;

            await KategorileriYukleAsync(
                item.KategoriId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guncelle(
            AnasayfaKategoriGuncelleDto model,
            IFormFile? gorselDosyasi)
        {
            if (gorselDosyasi != null)
            {
                try
                {
                    model.GorselUrl = await VitrinGorseliniKaydetAsync(gorselDosyasi);
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("gorselDosyasi", ex.Message);
                    await KategorileriYukleAsync(model.KategoriId);
                    return View(model);
                }
            }

            if (!ModelState.IsValid)
            {
                await KategorileriYukleAsync(
                    model.KategoriId);

                return View(model);
            }

            var client = CreateClientWithToken();

            var response =
                await client.PutAsJsonAsync(
                    "anasayfa-kategoriler",
                    model);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] =
                    "Anasayfa kategorisi güncellendi.";

                return RedirectToAction(nameof(Index));
            }

            var hata =
                await response.Content.ReadAsStringAsync();

            ModelState.AddModelError(
                string.Empty,
                hata);

            await KategorileriYukleAsync(
                model.KategoriId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(Guid id)
        {
            var client = CreateClientWithToken();

            var response =
                await client.DeleteAsync(
                    $"anasayfa-kategoriler/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] =
                    "Kategori anasayfadan kaldırıldı.";
            }
            else
            {
                TempData["HataMesaji"] =
                    "Kategori anasayfadan kaldırılırken hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sirala([FromBody] List<Guid> idler)
        {
            if (idler == null || !idler.Any())
                return BadRequest(new { mesaj = "Geçersiz veri." });

            var client = CreateClientWithToken();
            var response = await client.PutAsJsonAsync("anasayfa-kategoriler/sirala", idler);

            if (response.IsSuccessStatusCode)
            {
                return Ok(new { mesaj = "Kategori sıralaması güncellendi." });
            }

            return BadRequest(new { mesaj = "Sıralama güncellenemedi." });
        }

        private async Task KategorileriYukleAsync(
            Guid? seciliKategoriId = null)
        {
            var client = CreateClientWithToken();

            var response =
                await client.GetAsync("kategoriler");

            var kategoriler =
                new List<KategoriListeDto>();

            if (response.IsSuccessStatusCode)
            {
                var content =
                    await response.Content.ReadAsStringAsync();

                kategoriler =
                    JsonSerializer.Deserialize<
                        List<KategoriListeDto>>(
                        content,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        })
                    ?? new List<KategoriListeDto>();
            }

            var anasayfaResponse =
                await client.GetAsync(
                    "anasayfa-kategoriler");

            var kullanilanKategoriIdleri =
                new HashSet<Guid>();

            if (anasayfaResponse.IsSuccessStatusCode)
            {
                var content =
                    await anasayfaResponse.Content.ReadAsStringAsync();

                var anasayfaKategorileri =
                    JsonSerializer.Deserialize<
                        List<AnasayfaKategoriDto>>(
                        content,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                if (anasayfaKategorileri != null)
                {
                    kullanilanKategoriIdleri =
                        anasayfaKategorileri
                            .Where(x =>
                                !seciliKategoriId.HasValue ||
                                x.KategoriId != seciliKategoriId.Value)
                            .Select(x => x.KategoriId)
                            .ToHashSet();
                }
            }

            kategoriler = kategoriler
                .Where(x =>
                    x.AktifMi &&
                    !kullanilanKategoriIdleri.Contains(x.Id))
                .OrderBy(x => x.Ad)
                .ToList();

            ViewBag.Kategoriler =
                new SelectList(
                    kategoriler,
                    "Id",
                    "Ad",
                    seciliKategoriId);
        }

        private async Task<string> VitrinGorseliniKaydetAsync(IFormFile dosya)
        {
            if (dosya.Length == 0 || dosya.Length > 5 * 1024 * 1024)
                throw new InvalidOperationException("Görsel 5 MB'dan küçük olmalıdır.");

            if (!string.Equals(dosya.ContentType, "image/webp", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Lütfen kırpma penceresinden hazırlanmış bir görsel seçin.");

            return await ECommerce.WebUI.Services.ApiGorselYukleyici.YukleAsync(
                _httpClientFactory, User, dosya, "anasayfa-kategoriler");
        }
    }
}
