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

        var adminKullanici = await userManager.FindByEmailAsync("admin@lidya.com");

        if (adminKullanici == null && !userManager.Users.Any())
        {
            var yeniAdmin = new UygulamaKullanicisi
            {
                UserName = "admin@lidya.com",
                Email = "admin@lidya.com",
                Ad = "Sistem",
                Soyad = "Yöneticisi",
                EmailConfirmed = true
            };

            var sonuc = await userManager.CreateAsync(yeniAdmin, "Admin123!");
            if (sonuc.Succeeded)
                adminKullanici = yeniAdmin;
        }

        // Yetkilendirme veritabanındaki rollerle yapılır; admin hesabına Admin rolü yoksa
        // [Authorize(Roles = "Admin")] korumalı uçlara erişemez. Mevcut veritabanları için de tamamlanır.
        if (adminKullanici != null && !await userManager.IsInRoleAsync(adminKullanici, "Admin"))
        {
            await userManager.AddToRoleAsync(adminKullanici, "Admin");
        }
    }
}