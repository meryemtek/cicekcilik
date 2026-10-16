using System.Net;
using System.Net.Mail;
using ECommerce.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Services;

public class EmailServisi : IEmailServisi
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailServisi> _logger;

    public EmailServisi(IConfiguration configuration, ILogger<EmailServisi> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> EpostaGonderAsync(string aliciEmail, string konu, string htmlIcerik)
    {
        try
        {
            var host = _configuration["SmtpSettings:Host"];
            var portStr = _configuration["SmtpSettings:Port"];
            var kullanici = _configuration["SmtpSettings:KullaniciAdi"];
            // Google uygulama şifreleri kopyalanırken araya boşluk girebilir.
            // SMTP kimlik doğrulamasında bu boşluklar geçerli olmadığı için temizlenir.
            var sifre = _configuration["SmtpSettings:Sifre"]?.Replace(" ", string.Empty).Trim();
            var gonderenEmail = _configuration["SmtpSettings:GonderenEmail"] ?? "bilgi@lidyacicekcilik.com";
            var gonderenBaslik = _configuration["SmtpSettings:GonderenBaslik"] ?? "Lidya Çiçekçilik";
            var sslStr = _configuration["SmtpSettings:EnableSsl"];

            int.TryParse(portStr, out int port);
            if (port == 0) port = 587;
            bool.TryParse(sslStr, out bool enableSsl);

            // Gerçek SMTP bilgileri tanımlıysa doğrudan sunucudan gönder
            if (!string.IsNullOrWhiteSpace(host) && !string.IsNullOrWhiteSpace(kullanici) && !string.IsNullOrWhiteSpace(sifre))
            {
                using var client = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(kullanici, sifre),
                    EnableSsl = enableSsl
                };

                using var mail = new MailMessage
                {
                    From = new MailAddress(gonderenEmail, gonderenBaslik),
                    Subject = konu,
                    Body = htmlIcerik,
                    IsBodyHtml = true
                };

                mail.To.Add(aliciEmail);
                await client.SendMailAsync(mail);

                _logger.LogInformation("E-posta başarıyla gönderildi: {Email}", aliciEmail);
                return true;
            }
            else
            {
                // SMTP ayarları henüz girilmemişse simülasyon ve loglama yap
                _logger.LogWarning("SMTP ayarları henüz yapılandırılmamış. E-posta simüle edildi.");
                _logger.LogInformation("================== E-POSTA SİMÜLASYONU ==================");
                _logger.LogInformation("Alıcı: {Email}", aliciEmail);
                _logger.LogInformation("Konu: {Konu}", konu);
                _logger.LogInformation("İçerik: {Icerik}", htmlIcerik);
                _logger.LogInformation("=========================================================");
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "E-posta gönderilirken hata oluştu: {Email}", aliciEmail);
            return false;
        }
    }
}
