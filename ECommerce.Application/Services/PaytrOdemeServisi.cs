using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using ECommerce.Application.DTOs.Odeme;
using ECommerce.Application.Interfaces;

namespace ECommerce.Application.Services;

public class PaytrOdemeServisi : IOdemeServisi
{
    private readonly ISiteAyariServisi _siteAyariServisi;

    public PaytrOdemeServisi(ISiteAyariServisi siteAyariServisi)
    {
        _siteAyariServisi = siteAyariServisi;
    }

    public async Task<OdemeYanitDto> OdemeBaslatAsync(OdemeIstekDto istek)
    {
        // Bilgileri appsettings yerine doğrudan veritabanındaki site ayarlarından çekiyoruz
        var ayarlar = await _siteAyariServisi.AyarlariGetirAsync();

        if (ayarlar == null || string.IsNullOrEmpty(ayarlar.PaytrMerchantId))
        {
            return new OdemeYanitDto
            {
                BasariliMi = false,
                HataMesaji = "PayTR sanal pos bilgileri yönetim panelinden henüz yapılandırılmamış."
            };
        }

        var merchantId = ayarlar.PaytrMerchantId;
        var merchantKey = ayarlar.PaytrMerchantKey;
        var merchantSalt = ayarlar.PaytrMerchantSalt;

        // PayTR sepet formatı hazırlığı (Base64 çevrimi)
        var sepetListesi = new List<object>();
        foreach (var urun in istek.Urunler)
        {
            sepetListesi.Add(new object[] { urun.Adi, urun.Fiyat, urun.Adet });
        }
        var sepetBytes = System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(sepetListesi));
        var userBasket = Convert.ToBase64String(sepetBytes);

        // Gerçek entegrasyonda burada HttpClient ile PayTR sunucularına token talebi atılır.
        // Mimari iskeletinde şimdilik başarılı bir token dönecek şekilde kurguluyoruz:

        return new OdemeYanitDto
        {
            BasariliMi = true,
            TokenVeyaUrl = "paytr_iframe_token_ornegi_987654321",
            HataMesaji = string.Empty
        };
    }
}