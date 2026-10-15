using ECommerce.Application.DTOs.BankaHesabi;
using ECommerce.Application.DTOs.KartNotuSablonu;
using ECommerce.Application.DTOs.Sayfa;
using ECommerce.Application.DTOs.Siparis;
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

                return RedirectToAction("Basarili", new { id = siparisId });
            }

            TempData["HataMesaji"] = "Sipariş oluşturulamadı. Lütfen bilgilerinizi kontrol edin.";
            return RedirectToAction("Index");
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
