using System;
using System.Threading.Tasks;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Application.Services;

public class HiCariTakipServisi : IHiCariEntegrasyonServisi
{
    private readonly IDepo<Siparis> _siparisDeposu;
    private readonly IConfiguration _configuration;

    public HiCariTakipServisi(IDepo<Siparis> siparisDeposu, IConfiguration configuration)
    {
        _siparisDeposu = siparisDeposu;
        _configuration = configuration;
    }

    public async Task<bool> CariVeStokIsleAsync(Guid siparisId)
    {
        var siparis = await _siparisDeposu.IdIleGetirAsync(siparisId);
        if (siparis == null)
        {
            return false;
        }

        // HiCariTakip sistemine ait API bağlantı adresini veya ayarlarını konfigürasyondan çekiyoruz
        var hiCariApiUrl = _configuration["HiCariTakip:ApiUrl"];
        var apiKey = _configuration["HiCariTakip:ApiKey"];

        // 1. Adım: Müşteri bilgilerine göre HiCariTakip'te cari hesap kontrolü / oluşturulması
        // 2. Adım: Sipariş tutarının ilgili cari hesaba borç (satış) olarak kaydedilmesi
        // 3. Adım: Siparişteki kalemlerin (SiparisKalemleri) stok miktarlarından düşülmesi

        // Mimari iskeletinde bu operasyonun başarıyla gerçekleştiğini simüle ediyoruz:
        return await Task.FromResult(true);
    }
}