using System.Text;
using System.Xml.Linq;
using ECommerce.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Services;

public class NetGsmSmsServisi : ISmsServisi
{
    private readonly HttpClient _httpClient;
    private readonly ISiteAyariServisi _siteAyariServisi;
    private readonly ILogger<NetGsmSmsServisi> _logger;

    public NetGsmSmsServisi(HttpClient httpClient, ISiteAyariServisi siteAyariServisi,
        ILogger<NetGsmSmsServisi> logger)
    {
        _httpClient = httpClient;
        _siteAyariServisi = siteAyariServisi;
        _logger = logger;
    }

    public async Task<bool> SmsGonderAsync(string telefonNumarasi, string mesaj)
    {
        var telefon = TelefonuDuzenle(telefonNumarasi);
        if (telefon == null || string.IsNullOrWhiteSpace(mesaj))
        {
            _logger.LogWarning("SMS gönderilemedi: geçersiz telefon veya boş mesaj.");
            return false;
        }

        try
        {
            var ayarlar = await _siteAyariServisi.AyarlariGetirAsync();
            if (ayarlar == null || string.IsNullOrWhiteSpace(ayarlar.NetGsmKullaniciAdi)
                || string.IsNullOrWhiteSpace(ayarlar.NetGsmSifre)
                || string.IsNullOrWhiteSpace(ayarlar.NetGsmBaslik))
            {
                _logger.LogWarning("SMS gönderilemedi: Netgsm hesap bilgileri veya gönderici başlığı eksik.");
                return false;
            }

            // XElement, mesaj ve hesap bilgilerindeki XML özel karakterlerini güvenli biçimde kodlar.
            var xml = new XElement("mainbody",
                new XElement("header",
                    new XElement("company", new XAttribute("dil", "TR"), "Netgsm"),
                    new XElement("usercode", ayarlar.NetGsmKullaniciAdi.Trim()),
                    new XElement("password", ayarlar.NetGsmSifre),
                    new XElement("type", "1:n"),
                    new XElement("msgheader", ayarlar.NetGsmBaslik.Trim())),
                new XElement("body", new XElement("msg", mesaj), new XElement("no", telefon)));

            using var content = new StringContent(xml.ToString(SaveOptions.DisableFormatting), Encoding.UTF8, "application/xml");
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            using var response = await _httpClient.PostAsync("https://api.netgsm.com.tr/sms/send/xml", content, timeout.Token);
            var body = await response.Content.ReadAsStringAsync(timeout.Token);
            var parts = body.Trim().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

            // HTTP 200, sağlayıcının SMS'i kabul ettiği anlamına gelmez. Başarılı yanıt: 00 <iş numarası>.
            if (response.IsSuccessStatusCode && parts.Length == 2 && parts[0] == "00"
                && parts[1].All(c => c is >= '0' and <= '9'))
                return true;

            var code = parts.Length > 0 && parts[0].Length <= 3 && parts[0].All(char.IsDigit)
                ? parts[0] : "bilinmiyor";
            _logger.LogWarning("Netgsm SMS isteğini kabul etmedi. HTTP: {StatusCode}, Kod: {Code}", (int)response.StatusCode, code);
            return false;
        }
        catch (Exception ex)
        {
            // SMS hatası, kaydedilmiş sipariş işlemini başarısız göstermemeli; hesap ve mesaj verilerini loglama.
            _logger.LogWarning("Netgsm SMS işlemi tamamlanamadı. Hata türü: {ErrorType}", ex.GetType().Name);
            return false;
        }
    }

    private static string? TelefonuDuzenle(string? telefon)
    {
        if (string.IsNullOrWhiteSpace(telefon)
            || telefon.Any(c => !(c is >= '0' and <= '9') && !" +()-".Contains(c)))
            return null;

        var digits = new string(telefon.Where(c => c is >= '0' and <= '9').ToArray());
        if (digits.StartsWith("0090")) digits = digits[4..];
        else if (digits.Length == 12 && digits.StartsWith("90")) digits = digits[2..];
        else if (digits.Length == 11 && digits.StartsWith('0')) digits = digits[1..];

        return digits.Length == 10 && digits.StartsWith('5') ? "90" + digits : null;
    }
}
