using System.Globalization;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ECommerce.Application.DTOs.Odeme;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Enums;

namespace ECommerce.Application.Services;

public class PaytrOdemeServisi : IOdemeServisi
{
    private readonly ISiteAyariServisi _siteAyariServisi;
    private readonly ISiparisDeposu _siparisDeposu;
    private readonly HttpClient _httpClient;

    public PaytrOdemeServisi(ISiteAyariServisi siteAyariServisi, ISiparisDeposu siparisDeposu, HttpClient httpClient)
    {
        _siteAyariServisi = siteAyariServisi;
        _siparisDeposu = siparisDeposu;
        _httpClient = httpClient;
    }

    public async Task<OdemeYanitDto> OdemeBaslatAsync(OdemeIstekDto istek)
    {
        var siparis = await _siparisDeposu.SiparisNumarasiylaDetayGetirAsync(istek.SiparisNumarasi);
        if (siparis == null || !string.Equals(siparis.OdemeYontemi, "KrediKarti", StringComparison.OrdinalIgnoreCase)
            || siparis.Durum != SiparisDurumu.Beklemede)
        {
            return new OdemeYanitDto { HataMesaji = "Ödeme için geçerli ve bekleyen bir sipariş bulunamadı." };
        }

        var ayarlar = await _siteAyariServisi.AyarlariGetirAsync();
        var merchantId = ayarlar?.PaytrMerchantId?.Trim();
        var merchantKey = ayarlar?.PaytrMerchantKey?.Trim();
        var merchantSalt = ayarlar?.PaytrMerchantSalt?.Trim();

        if (string.IsNullOrWhiteSpace(merchantId) || string.IsNullOrWhiteSpace(merchantKey)
            || string.IsNullOrWhiteSpace(merchantSalt))
        {
            return new OdemeYanitDto
            {
                BasariliMi = true,
                DemoModu = true,
                TokenVeyaUrl = "demo-paytr",
                HataMesaji = "PayTR bilgileri henüz girilmediği için demo ödeme ekranı açılıyor."
            };
        }

        if (siparis.ToplamTutar <= 0
            || !Uri.TryCreate(istek.MerchantOkUrl, UriKind.Absolute, out _)
            || !Uri.TryCreate(istek.MerchantFailUrl, UriKind.Absolute, out _)
            || !Uri.TryCreate(istek.NotificationUrl, UriKind.Absolute, out _))
        {
            return new OdemeYanitDto { HataMesaji = "PayTR ödeme isteği için sipariş ve dönüş adresleri eksik." };
        }

        var paymentAmount = decimal.Round(siparis.ToplamTutar * 100, 0, MidpointRounding.AwayFromZero)
            .ToString("0", CultureInfo.InvariantCulture);
        var basket = siparis.SiparisKalemleri.Select(x => new object[]
        {
            x.VaryantAdi == null ? x.Urun?.Ad ?? "Ürün" : $"{x.Urun?.Ad ?? "Ürün"} - {x.VaryantAdi}",
            x.BirimFiyat.ToString("0.00", CultureInfo.InvariantCulture),
            x.Miktar
        }).ToArray();
        var basketBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(basket)));
        var noInstallment = "0";
        var maxInstallment = "0";
        var currency = "TL";
        var testMode = "1";
        var hashText = string.Concat(merchantId, istek.IPAdresi, siparis.SiparisNumarasi,
            siparis.MusteriEmail, paymentAmount, basketBase64, noInstallment, maxInstallment, currency, testMode, merchantSalt);
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(merchantKey));
        var token = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(hashText)));

        var form = new Dictionary<string, string>
        {
            ["merchant_id"] = merchantId,
            ["user_ip"] = istek.IPAdresi,
            ["merchant_oid"] = siparis.SiparisNumarasi,
            ["email"] = siparis.MusteriEmail,
            ["payment_amount"] = paymentAmount,
            ["user_basket"] = basketBase64,
            ["no_installment"] = noInstallment,
            ["max_installment"] = maxInstallment,
            ["user_name"] = $"{siparis.MusteriAd} {siparis.MusteriSoyad}".Trim(),
            ["user_address"] = "Teslimat adresi sipariş kaydında.",
            ["user_phone"] = siparis.MusteriTelefon,
            ["merchant_ok_url"] = istek.MerchantOkUrl,
            ["merchant_fail_url"] = istek.MerchantFailUrl,
            ["merchant_notify_url"] = istek.NotificationUrl,
            ["currency"] = currency,
            ["test_mode"] = testMode,
            ["debug_on"] = "1",
            ["timeout_limit"] = "30",
            ["lang"] = "tr",
            ["paytr_token"] = token
        };

        using var response = await _httpClient.PostAsync("https://www.paytr.com/odeme/api/get-token", new FormUrlEncodedContent(form));
        var result = await response.Content.ReadFromJsonAsync<PaytrTokenResponse>();

        if (!response.IsSuccessStatusCode || result?.Status != "success" || string.IsNullOrWhiteSpace(result.Token))
            return new OdemeYanitDto { HataMesaji = $"PayTR ödeme formu başlatılamadı: {result?.Reason ?? "PayTR yanıt vermedi."}" };

        return new OdemeYanitDto { BasariliMi = true, TokenVeyaUrl = $"https://www.paytr.com/odeme/guvenli/{result.Token}" };
    }

    public async Task<bool> PaytrBildirimHashGecerliMiAsync(string siparisNumarasi, string durum, string toplamTutar, string hash)
    {
        var ayarlar = await _siteAyariServisi.AyarlariGetirAsync();
        var merchantKey = ayarlar?.PaytrMerchantKey?.Trim();
        var merchantSalt = ayarlar?.PaytrMerchantSalt?.Trim();
        if (string.IsNullOrWhiteSpace(merchantKey) || string.IsNullOrWhiteSpace(merchantSalt)
            || string.IsNullOrWhiteSpace(siparisNumarasi) || string.IsNullOrWhiteSpace(durum)
            || string.IsNullOrWhiteSpace(toplamTutar) || string.IsNullOrWhiteSpace(hash))
        {
            return false;
        }

        var hashText = string.Concat(siparisNumarasi, merchantSalt, durum, toplamTutar);
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(merchantKey));
        var expectedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(hashText)));
        var expectedBytes = Encoding.UTF8.GetBytes(expectedHash);
        var suppliedBytes = Encoding.UTF8.GetBytes(hash);
        return expectedBytes.Length == suppliedBytes.Length
            && CryptographicOperations.FixedTimeEquals(expectedBytes, suppliedBytes);
    }

    private sealed class PaytrTokenResponse
    {
        public string Status { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string? Reason { get; set; }
    }
}
