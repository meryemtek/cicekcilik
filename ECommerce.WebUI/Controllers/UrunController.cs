using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using ECommerce.Application.DTOs.Urun;
using ECommerce.Application.DTOs.UrunYorum;
using ECommerce.Application.DTOs.KartNotuSablonu;

namespace ECommerce.WebUI.Controllers
{
    public class UrunController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UrunController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Api'den gelen bölge listesini karşılamak için özel DTO
        private class TeslimatBolgeTempDto
        {
            public string IlAdi { get; set; } = string.Empty;
            public string IlceAdi { get; set; } = string.Empty;
            public string MahalleAdi { get; set; } = string.Empty;
            public decimal EkTeslimatUcreti { get; set; }
            public bool AktifMi { get; set; }
        }

        private string GetFullImageUrl(string gorselUrl)
        {
            if (string.IsNullOrWhiteSpace(gorselUrl)) return string.Empty;
            if (gorselUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return gorselUrl;

            var client = _httpClientFactory.CreateClient("ApiClient");
            var baseUrl = client.BaseAddress?.GetLeftPart(UriPartial.Authority) ?? "https://localhost:7001"; // API'nin çalıştığı port

            return $"{baseUrl.TrimEnd('/')}/{gorselUrl.TrimStart('/')}".Replace("\\", "/");
        }

        [HttpGet("urun/{seoUrl}")]
        public async Task<IActionResult> Detay(string seoUrl)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"urunler/{seoUrl}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var content = await response.Content.ReadAsStringAsync();
            var urun = JsonSerializer.Deserialize<UrunDetayDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // GÖRSEL YOLUNU API ADRESİ İLE BİRLEŞTİRİYORUZ
            if (urun != null)
            {
                urun.GorselUrl = GetFullImageUrl(urun.GorselUrl);

                if (urun.EkGorseller != null)
                {
                    urun.EkGorseller = urun.EkGorseller
                        .Where(g => !string.IsNullOrWhiteSpace(g))
                        .Select(g => GetFullImageUrl(g))
                        .ToList();
                }

                if (urun.TumGorseller != null && urun.TumGorseller.Any())
                {
                    urun.TumGorseller = urun.TumGorseller
                        .Where(g => !string.IsNullOrWhiteSpace(g))
                        .Select(g => GetFullImageUrl(g))
                        .Distinct()
                        .ToList();
                }
                else
                {
                    urun.TumGorseller = new List<string>();
                    if (!string.IsNullOrWhiteSpace(urun.GorselUrl))
                    {
                        urun.TumGorseller.Add(urun.GorselUrl);
                    }
                    if (urun.EkGorseller != null)
                    {
                        foreach (var eg in urun.EkGorseller)
                        {
                            if (!urun.TumGorseller.Contains(eg))
                            {
                                urun.TumGorseller.Add(eg);
                            }
                        }
                    }
                }
            }

            // AKTİF TESLİMAT BÖLGELERİNİ ÇEKME (İL, İLÇE, MAHALLE VE FİYATLAR)
            try
            {
                var bolgeResponse = await client.GetAsync("admin/semt-teslimat");
                if (bolgeResponse.IsSuccessStatusCode)
                {
                    var bolgeContent = await bolgeResponse.Content.ReadAsStringAsync();
                    var bolgeler = JsonSerializer.Deserialize<List<ECommerce.Application.DTOs.SemtTeslimatBolgesi.SemtBolgeListeDto>>(bolgeContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    var aktifBolgeler = bolgeler?.Where(x => x.AktifMi && (string.IsNullOrEmpty(x.IlAdi) || x.IlAdi.Equals("Elazığ", StringComparison.OrdinalIgnoreCase))).OrderBy(x => x.MahalleAdi).ToList() ?? new List<ECommerce.Application.DTOs.SemtTeslimatBolgesi.SemtBolgeListeDto>();

                    ViewBag.TeslimatBolgeleri = aktifBolgeler;

                    // JavaScript tarafında kolayca okuyabilmek için camelCase formatında JSON'a çeviriyoruz
                    var jsonAyarlar = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                    ViewBag.TeslimatBolgeleriJson = JsonSerializer.Serialize(aktifBolgeler, jsonAyarlar);
                }
                else
                {
                    ViewBag.TeslimatBolgeleri = new List<ECommerce.Application.DTOs.SemtTeslimatBolgesi.SemtBolgeListeDto>();
                    ViewBag.TeslimatBolgeleriJson = "[]";
                }
            }
            catch
            {
                ViewBag.TeslimatBolgeleriJson = "[]";
            }

            // AKTİF TESLİMAT SAATLERİNİ ÇEKME (Normal ve VIP)
            try
            {
                var saatResponse = await client.GetAsync("teslimatsaatleri/aktif");
                if (saatResponse.IsSuccessStatusCode)
                {
                    var saatContent = await saatResponse.Content.ReadAsStringAsync();
                    var teslimatSaatleri = JsonSerializer.Deserialize<List<ECommerce.Application.DTOs.TeslimatSaati.TeslimatSaatiDto>>(saatContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ECommerce.Application.DTOs.TeslimatSaati.TeslimatSaatiDto>();
                    ViewBag.TeslimatSaatleri = teslimatSaatleri;
                }
                else
                {
                    ViewBag.TeslimatSaatleri = new List<ECommerce.Application.DTOs.TeslimatSaati.TeslimatSaatiDto>();
                }
            }
            catch
            {
                ViewBag.TeslimatSaatleri = new List<ECommerce.Application.DTOs.TeslimatSaati.TeslimatSaatiDto>();
            }

            // SİTE AYARLARINI ÇEKME 
            try
            {
                var ayarlarResponse = await client.GetAsync("siteayarlari/site-ayarlari");
                if (ayarlarResponse.IsSuccessStatusCode)
                {
                    var ayarlarContent = await ayarlarResponse.Content.ReadAsStringAsync();

                    using (var doc = JsonDocument.Parse(ayarlarContent))
                    {
                        var root = doc.RootElement;

                        // Telefon1'i WhatsApp Numarası olarak kullanıyoruz
                        if (root.TryGetProperty("telefon1", out var wpElement) && wpElement.ValueKind != JsonValueKind.Null)
                        {
                            var tel = wpElement.GetString();
                            if (!string.IsNullOrEmpty(tel))
                            {
                                // Boşlukları ve özel karakterleri temizle
                                tel = tel.Replace(" ", "").Replace("+", "").Replace("(", "").Replace(")", "").Replace("-", "");

                                // Numarayı WhatsApp'a uygun hale getir (Başında 90 yoksa ekle)
                                if (tel.StartsWith("0"))
                                {
                                    tel = "90" + tel.Substring(1); // 0555 -> 90555
                                }
                                else if (tel.Length == 10)
                                {
                                    tel = "90" + tel; // 555 -> 90555
                                }

                                ViewBag.WhatsappNo = tel;
                            }
                        }

                        // Kayan bant duyurularını alıyoruz
                        if (root.TryGetProperty("urunDetayDuyurulari", out var duyuruElement) && duyuruElement.ValueKind != JsonValueKind.Null)
                        {
                            var duyuruMetni = duyuruElement.GetString();
                            if (!string.IsNullOrEmpty(duyuruMetni))
                            {
                                ViewBag.Duyurular = duyuruMetni
                                    .Split(new[] { ',', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(x => x.Trim())
                                    .ToList();
                            }
                        }

                        // ====================================================================
                        // VIP TESLİMAT ÜCRETİ - KÜLTÜR (LOCALE) HATASINI ENGELLEYEN PARSE
                        // ====================================================================
                        if (root.TryGetProperty("vipTeslimatUcreti", out var vipElement) && vipElement.ValueKind != JsonValueKind.Null)
                        {
                            // API'den gelen değeri string olarak alıp noktayı/virgülü InvariantCulture ile eziyoruz
                            string vipStr = vipElement.ToString().Replace(",", ".");
                            if (decimal.TryParse(vipStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal gercekVipUcreti))
                            {
                                ViewBag.VipUcreti = gercekVipUcreti;
                            }
                        }
                        // ====================================================================
                    }
                }
            }
            catch { /* API yanıt vermezse Detay.cshtml içindeki varsayılanlar kullanılır */ }

            // AKTİF KART NOTU ŞABLONLARINI ÇEKME
            try
            {
                var notlarResponse = await client.GetAsync("kart-notlari/aktif");
                if (notlarResponse.IsSuccessStatusCode)
                {
                    var notlarContent = await notlarResponse.Content.ReadAsStringAsync();
                    var kartNotlari = JsonSerializer.Deserialize<List<KartNotuSablonuListeDto>>(notlarContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<KartNotuSablonuListeDto>();
                    ViewBag.KartNotlari = kartNotlari;
                }
                else
                {
                    ViewBag.KartNotlari = new List<KartNotuSablonuListeDto>();
                }
            }
            catch
            {
                ViewBag.KartNotlari = new List<KartNotuSablonuListeDto>();
            }

            return View(urun);
        }

        [HttpPost("urun/yorum-ekle")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> YorumEkle(UrunYorumEkleDto dto, string seoUrl)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var jsonContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("urunler/yorum-ekle", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["BasariliMesaj"] = "Yorumunuz başarıyla gönderildi. Onaylandıktan sonra yayınlanacaktır.";
            }
            else
            {
                TempData["HataMesaji"] = "Yorum gönderilirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Detay), new { seoUrl });
        }

        [HttpGet("urunler")]
        [HttpGet("Arama")]
        public async Task<IActionResult> Urunler(string? q)
        {
            var aramaMetni = q?.Trim();
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("urunler");

            var urunler = new List<UrunListeDto>();
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                urunler = JsonSerializer.Deserialize<List<UrunListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

                foreach (var urun in urunler)
                {
                    urun.GorselUrl = GetFullImageUrl(urun.GorselUrl);
                }
            }

            if (!string.IsNullOrWhiteSpace(aramaMetni))
            {
                urunler = urunler.Where(x =>
                    x.Ad.Contains(aramaMetni, StringComparison.OrdinalIgnoreCase) ||
                    (x.UrunKodu?.Contains(aramaMetni, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();
            }

            ViewBag.AramaMetni = aramaMetni;
            return View(urunler);
        }

        [HttpGet("kategori/{seoUrl}")]
        public async Task<IActionResult> KategoriDetay(string seoUrl)
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            // 1. Tüm kategorileri çekip gelen SeoUrl ile eşleşeni ve ID'sini buluyoruz
            var katResponse = await client.GetAsync("kategoriler");
            string kategoriAdi = "Kategori Ürünleri";
            Guid? kategoriId = null;

            if (katResponse.IsSuccessStatusCode)
            {
                var katContent = await katResponse.Content.ReadAsStringAsync();
                var kategoriler = JsonSerializer.Deserialize<List<ECommerce.Application.DTOs.Kategori.KategoriListeDto>>(katContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var aktifKat = kategoriler?.FirstOrDefault(x => x.SeoUrl != null && x.SeoUrl.Equals(seoUrl, StringComparison.OrdinalIgnoreCase));
                if (aktifKat != null)
                {
                    kategoriAdi = aktifKat.Ad;
                    kategoriId = aktifKat.Id;
                }
            }
            ViewBag.KategoriAdi = kategoriAdi;

            // 2. Bulunan Kategori ID'si ile ürünleri API'den çekiyoruz
            var urunler = new List<ECommerce.Application.DTOs.Urun.UrunListeDto>();
            if (kategoriId.HasValue)
            {
                var response = await client.GetAsync($"urunler/kategori/{kategoriId.Value}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    urunler = JsonSerializer.Deserialize<List<ECommerce.Application.DTOs.Urun.UrunListeDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

                    foreach (var urun in urunler)
                    {
                        urun.GorselUrl = GetFullImageUrl(urun.GorselUrl);
                    }
                }
            }

            return View(urunler);
        }
    }
}
