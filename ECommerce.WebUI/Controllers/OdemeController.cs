using ECommerce.Application.DTOs.BankaHesabi;
using ECommerce.Application.DTOs.KartNotuSablonu;
using ECommerce.Application.DTOs.Sayfa;
using ECommerce.Application.DTOs.Siparis;
using ECommerce.Application.DTOs.Odeme;
using ECommerce.Application.Models;
using ECommerce.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace ECommerce.WebUI.Controllers
{
    public class OdemeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string SepetCookieName = "LidyaSepetId";

        public OdemeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private string GetOrCreateSepetId()
        {
            if (Request.Cookies.TryGetValue(SepetCookieName, out var sepetId) && !string.IsNullOrEmpty(sepetId))
            {
                return sepetId;
            }
            return "sep_" + Guid.NewGuid().ToString();
        }

        private HttpClient CreateApiClient()
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
        public async Task<IActionResult> Index()
        {
            var sepetId = GetOrCreateSepetId();
            var client = CreateApiClient();

            // 1. Sepeti Çek
            var sepetResponse = await client.GetAsync($"sepet/{sepetId}");
            if (!sepetResponse.IsSuccessStatusCode)
            {
                TempData["HataMesaji"] = "Sepet bilgileri alınamadı.";
                return RedirectToAction("Index", "Sepet");
            }
            var sepetContent = await sepetResponse.Content.ReadAsStringAsync();
            var sepet = JsonSerializer.Deserialize<Sepet>(sepetContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (sepet == null || sepet.Kalemler == null || !sepet.Kalemler.Any())
            {
                TempData["HataMesaji"] = "Sepetiniz boş olduğu için ödeme sayfasına geçemezsiniz.";
                return RedirectToAction("Index", "Sepet");
            }

            // 2. Aktif Kart Notu Şablonlarını Çek
            var notlarResponse = await client.GetAsync("kart-notlari/aktif");
            var kartNotlari = new List<KartNotuSablonuListeDto>();

            if (notlarResponse.IsSuccessStatusCode)
            {
                var notlarContent = await notlarResponse.Content.ReadAsStringAsync();
                kartNotlari = JsonSerializer.Deserialize<List<KartNotuSablonuListeDto>>(notlarContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            // 3. Aktif Banka Hesaplarını Çek
            var bankaResponse = await client.GetAsync("banka-hesaplari/aktif");
            var bankalar = new List<BankaHesabiPublicDto>();
            if (bankaResponse.IsSuccessStatusCode)
            {
                var bankaContent = await bankaResponse.Content.ReadAsStringAsync();
                bankalar = JsonSerializer.Deserialize<List<BankaHesabiPublicDto>>(bankaContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            // 4. İki Sayfayı ID Çakışması Olmayacak Şekilde Kesin Olarak Ayırarak Çekme
            string mesafeliBaslik = "Mesafeli Satış Sözleşmesi";
            string mesafeliSatisMetni = "Mesafeli satış sözleşmesi içeriği yüklenemedi.";

            string gizlilikBaslik = "Gizlilik Politikası";
            string gizlilikMetni = "Gizlilik politikası içeriği yüklenemedi.";

            var aktifListeResponse = await client.GetAsync("sayfalar/aktif-liste");
            if (aktifListeResponse.IsSuccessStatusCode)
            {
                var aktifSayfalar = await aktifListeResponse.Content.ReadFromJsonAsync<List<SayfaListeDto>>();
                if (aktifSayfalar != null && aktifSayfalar.Any())
                {
                    // --- 1. ADIM: MESAFELİ SATIŞ SAYFASINI BUL ---
                    var mesafeliOzet = aktifSayfalar.FirstOrDefault(x =>
                        (x.SayfaKodu != null && x.SayfaKodu.Contains("mesafeli", StringComparison.OrdinalIgnoreCase)) ||
                        (x.Baslik != null && (x.Baslik.Contains("mesafeli", StringComparison.OrdinalIgnoreCase) || x.Baslik.Contains("satış", StringComparison.OrdinalIgnoreCase) || x.Baslik.Contains("satis", StringComparison.OrdinalIgnoreCase))));

                    Guid? mesafeliId = mesafeliOzet?.Id;

                    if (mesafeliOzet != null && !string.IsNullOrEmpty(mesafeliOzet.SayfaKodu))
                    {
                        var detayRes = await client.GetAsync($"sayfalar/{Uri.EscapeDataString(mesafeliOzet.SayfaKodu)}");
                        if (detayRes.IsSuccessStatusCode)
                        {
                            var detayDto = await detayRes.Content.ReadFromJsonAsync<SayfaGuncelleDto>();
                            if (detayDto != null)
                            {
                                mesafeliBaslik = detayDto.Baslik;
                                mesafeliSatisMetni = detayDto.Icerik;
                            }
                        }
                    }

                    // --- 2. ADIM: GİZLİLİK POLİTİKASINI BUL (Mesafeli satışın ID'si HARİÇ tutularak!) ---
                    var gizlilikOzet = aktifSayfalar.FirstOrDefault(x =>
                        x.Id != mesafeliId && (
                            (x.SayfaKodu != null && x.SayfaKodu.Contains("gizlilik", StringComparison.OrdinalIgnoreCase)) ||
                            (x.Baslik != null && (x.Baslik.Contains("gizlilik", StringComparison.OrdinalIgnoreCase) || x.Baslik.Contains("politika", StringComparison.OrdinalIgnoreCase)))
                        ));

                    if (gizlilikOzet != null && !string.IsNullOrEmpty(gizlilikOzet.SayfaKodu))
                    {
                        var detayRes = await client.GetAsync($"sayfalar/{Uri.EscapeDataString(gizlilikOzet.SayfaKodu)}");
                        if (detayRes.IsSuccessStatusCode)
                        {
                            var detayDto = await detayRes.Content.ReadFromJsonAsync<SayfaGuncelleDto>();
                            if (detayDto != null)
                            {
                                gizlilikBaslik = detayDto.Baslik;
                                gizlilikMetni = detayDto.Icerik;
                            }
                        }
                    }
                }
            }

            var viewModel = new OdemeViewModel
            {
                Sepet = sepet,
                KartNotlari = kartNotlari,
                Bankalar = bankalar,
                MesafeliSatisBaslik = mesafeliBaslik,
                MesafeliSatisIcerigi = mesafeliSatisMetni,
                GizlilikBaslik = gizlilikBaslik,
                GizlilikPolitikasiIcerigi = gizlilikMetni
            };

            if (!string.IsNullOrEmpty(Request.Cookies["JWToken"]))
            {
                var adresResponse = await client.GetAsync("hesap/adresler");
                if (adresResponse.IsSuccessStatusCode)
                    viewModel.KayitliAdresler = await adresResponse.Content.ReadFromJsonAsync<List<ECommerce.Application.DTOs.Adres.KayitliAdresDto>>() ?? new();
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Tamamla(
            string musteriAdSoyad, string musteriEmail, string musteriTelefon,
            string faturaAdresi, string faturaTipi, string? firmaAdi, string? vergiNo,
            string aliciAdSoyad, string aliciTelefon, string gonderimYeriAdi, string teslimatAdresi,
            string? kartNotu, string? gonderenGorunenAd, string odemeYontemi)
        {
            var sepetId = GetOrCreateSepetId();
            var client = CreateApiClient();

            var sepetResponse = await client.GetAsync($"sepet/{sepetId}");
            if (!sepetResponse.IsSuccessStatusCode)
            {
                TempData["HataMesaji"] = "Sepet bilgisine ulaşılamadı.";
                return RedirectToAction("Index", "Sepet");
            }

            var sepetContent = await sepetResponse.Content.ReadAsStringAsync();
            var sepet = JsonSerializer.Deserialize<Sepet>(sepetContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (sepet == null || !sepet.Kalemler.Any())
            {
                TempData["HataMesaji"] = "Sepetiniz boş.";
                return RedirectToAction("Index", "Sepet");
            }

            var adSoyadParca = musteriAdSoyad.Trim().Split(' ', 2);
            string ad = adSoyadParca[0];
            string soyad = adSoyadParca.Length > 1 ? adSoyadParca[1] : "";

            var dto = new SiparisOlusturDto
            {
                SepetId = sepetId,
                MusteriAd = ad,
                MusteriSoyad = soyad,
                MusteriEmail = musteriEmail,
                MusteriTelefon = musteriTelefon,
                FaturaAdresi = faturaAdresi,
                FaturaTipi = faturaTipi,
                FirmaAdi = firmaAdi,
                VergiNo = vergiNo,
                AliciAdSoyad = aliciAdSoyad,
                AliciTelefon = aliciTelefon,
                GonderimYeriAdi = gonderimYeriAdi,
                TeslimatAdresi = teslimatAdresi,
                KartNotu = kartNotu,
                GonderenGorunenAd = gonderenGorunenAd,
                OdemeYontemi = odemeYontemi,
                // API, giriş yapan müşterinin kimliğini doğrulanmış JWT üzerinden belirler.
                KullaniciId = null
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("siparis/olustur", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseContent);
                var siparisId = doc.RootElement.GetProperty("siparisId").GetGuid();

                if (string.Equals(odemeYontemi, "KrediKarti", StringComparison.OrdinalIgnoreCase))
                {
                    var detayResponse = await client.GetAsync($"siparis/benim-siparislerim/{siparisId}");
                    if (detayResponse.IsSuccessStatusCode)
                    {
                        var detay = await detayResponse.Content.ReadFromJsonAsync<SiparisDetayDto>();
                        if (detay != null)
                        {
                            var odemeIstek = new OdemeIstekDto
                            {
                                SiparisNumarasi = detay.SiparisNumarasi,
                                ToplamTutar = detay.ToplamTutar,
                                MusteriEmail = detay.MusteriEmail,
                                MusteriAdSoyad = $"{detay.MusteriAd} {detay.MusteriSoyad}".Trim(),
                                MusteriTelefon = detay.MusteriTelefon,
                                IPAdresi = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1",
                                MerchantOkUrl = Url.Action(nameof(Basarili), "Odeme", new { id = siparisId }, Request.Scheme) ?? string.Empty,
                                MerchantFailUrl = Url.Action(nameof(Index), "Odeme", null, Request.Scheme) ?? string.Empty,
                                NotificationUrl = Url.Action("Bildirim", "Odeme", null, Request.Scheme) ?? string.Empty,
                                Urunler = detay.Kalemler.Select(k => new OdemeUrunDto { Adi = k.UrunAdi, Fiyat = k.BirimFiyat, Adet = k.Miktar }).ToList()
                            };

                            var paymentResponse = await client.PostAsJsonAsync("odeme/baslat", odemeIstek);
                            var payment = await paymentResponse.Content.ReadFromJsonAsync<OdemeYanitDto>();
                            if (payment?.BasariliMi == true)
                            {
                                TempData["PaytrToken"] = payment.TokenVeyaUrl;
                                return RedirectToAction(payment.DemoModu ? nameof(Demo) : nameof(Paytr), new { id = siparisId });
                            }

                            TempData["HataMesaji"] = payment?.HataMesaji ?? "Ödeme ekranı başlatılamadı.";
                            await SiparisOdemeBaslatmasiniIptalEtAsync(client, siparisId);
                            return RedirectToAction(nameof(Index));
                        }
                    }

                    await SiparisOdemeBaslatmasiniIptalEtAsync(client, siparisId);
                    TempData["HataMesaji"] = "Sipariş bilgileri alınamadığı için ödeme başlatılamadı. Sepetiniz korunmuştur.";
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction("Basarili", new { id = siparisId });
            }

            string errorMessage = "Sipariş oluşturulamadı. Lütfen bilgilerinizi kontrol edin.";
            try
            {
                var errContent = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(errContent);
                if (doc.RootElement.TryGetProperty("mesaj", out var msgProp) && !string.IsNullOrWhiteSpace(msgProp.GetString()))
                {
                    errorMessage = msgProp.GetString()!;
                }
            }
            catch { }

            TempData["HataMesaji"] = errorMessage;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Bildirim(
            [FromForm(Name = "merchant_oid")] string merchantOid,
            [FromForm(Name = "status")] string status,
            [FromForm(Name = "total_amount")] string totalAmount,
            [FromForm(Name = "hash")] string hash)
        {
            var form = new Dictionary<string, string>
            {
                ["merchant_oid"] = merchantOid ?? string.Empty,
                ["status"] = status ?? string.Empty,
                ["total_amount"] = totalAmount ?? string.Empty,
                ["hash"] = hash ?? string.Empty
            };

            using var response = await CreateApiClient().PostAsync("odeme/bildirim", new FormUrlEncodedContent(form));
            var content = await response.Content.ReadAsStringAsync();
            return new ContentResult
            {
                Content = content,
                ContentType = "text/plain; charset=utf-8",
                StatusCode = (int)response.StatusCode
            };
        }

        [HttpGet]
        public async Task<IActionResult> Demo(Guid id)
        {
            var siparis = await SiparisDetayiniGetirAsync(id);
            if (siparis == null) return NotFound();
            return View(siparis);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DemoTamamla(Guid id)
        {
            var sepetId = GetOrCreateSepetId();
            await CreateApiClient().DeleteAsync($"sepet/{sepetId}/temizle");
            return RedirectToAction(nameof(Basarili), new { id });
        }

        private static async Task SiparisOdemeBaslatmasiniIptalEtAsync(HttpClient client, Guid siparisId)
        {
            await client.PostAsync($"siparis/{siparisId}/odeme-baslatma-iptal", null);
        }

        [HttpGet]
        public async Task<IActionResult> Paytr(Guid id)
        {
            var siparis = await SiparisDetayiniGetirAsync(id);
            var token = TempData["PaytrToken"] as string;
            if (siparis == null || string.IsNullOrWhiteSpace(token)) return RedirectToAction(nameof(Index));
            ViewBag.PaytrUrl = token;
            return View(siparis);
        }

        private async Task<SiparisDetayDto?> SiparisDetayiniGetirAsync(Guid id)
        {
            var response = await CreateApiClient().GetAsync($"siparis/benim-siparislerim/{id}");
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<SiparisDetayDto>()
                : null;
        }

        [HttpGet]
        public async Task<IActionResult> Basarili(Guid id)
        {
            var client = CreateApiClient();
            var response = await client.GetAsync($"siparis/benim-siparislerim/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var siparis = JsonSerializer.Deserialize<SiparisDetayDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(siparis);
            }

            return View(new SiparisDetayDto { Id = id, SiparisNumarasi = "SP-XXXX" });
        }
    }
}
