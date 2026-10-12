using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ECommerce.Application.Interfaces;

namespace ECommerce.Application.Services;

public class NetGsmSmsServisi : ISmsServisi
{
    private readonly HttpClient _httpClient;
    private readonly ISiteAyariServisi _siteAyariServisi;

    public NetGsmSmsServisi(HttpClient httpClient, ISiteAyariServisi siteAyariServisi)
    {
        _httpClient = httpClient;
        _siteAyariServisi = siteAyariServisi;
    }

    public async Task<bool> SmsGonderAsync(string telefonNumarasi, string mesaj)
    {
        // Bilgileri appsettings yerine doğrudan veritabanındaki site ayarlarından çekiyoruz
        var ayarlar = await _siteAyariServisi.AyarlariGetirAsync();

        if (ayarlar == null || string.IsNullOrEmpty(ayarlar.NetGsmKullaniciAdi) || string.IsNullOrEmpty(ayarlar.NetGsmSifre))
        {
            // Admin panelinden henüz NetGSM bilgileri girilmemişse işlem yapma
            return false;
        }

        var netgsmRequest = new
        {
            auth = new
            {
                username = ayarlar.NetGsmKullaniciAdi,
                password = ayarlar.NetGsmSifre
            },
            header = ayarlar.NetGsmBaslik,
            row = new[]
            {
                new
                {
                    no = telefonNumarasi,
                    msg = mesaj
                }
            }
        };

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(netgsmRequest),
            Encoding.UTF8,
            "application/json");

        try
        {
            var response = await _httpClient.PostAsync("https://api.netgsm.com.tr/json/send/", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
        }
        catch (Exception)
        {
            return false;
        }

        return false;
    }
}