using System.Linq;
using System.Threading.Tasks;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.WebAPI.Extensions;

public static class SeedDataExtensions
{
    public static async Task SeedVerileriOlusturAsync(this IApplicationBuilder app)
    {
        // Uygulamanın servislerine erişim için bir kapsam (scope) oluşturuyoruz
        using var scope = app.ApplicationServices.CreateScope();

        // 1. Site Ayarları Seed İşlemi
        var siteAyariDeposu = scope.ServiceProvider.GetRequiredService<IDepo<SiteAyari>>();
        var mevcutAyarlar = await siteAyariDeposu.TumunuGetirAsync();

        if (!mevcutAyarlar.Any())
        {
            var ilkAyar = new SiteAyari
            {
                FirmaAdi = "Lidya Çiçekçilik",
                Email = "info@lidyacicekcilik.com",
                Telefon1 = "0850 000 00 00",
                Adres = "İstanbul, Türkiye",
                TemaRengiHex = "#2e8b57",
                NetGsmKullaniciAdi = "",
                NetGsmSifre = "",
                NetGsmBaslik = "",
                PaytrMerchantId = "",
                PaytrMerchantKey = "",
                PaytrMerchantSalt = ""
            };

            await siteAyariDeposu.EkleAsync(ilkAyar);
            await siteAyariDeposu.DegisiklikleriKaydetAsync();
        }

        // 2. Varsayılan Admin Kullanıcısı Seed İşlemi
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UygulamaKullanicisi>>();

        if (!userManager.Users.Any())
        {
            var adminKullanici = new UygulamaKullanicisi
            {
                UserName = "admin@lidya.com",
                Email = "admin@lidya.com",
                Ad = "Sistem",
                Soyad = "Yöneticisi",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(adminKullanici, "Admin123!");
        }
    }
}