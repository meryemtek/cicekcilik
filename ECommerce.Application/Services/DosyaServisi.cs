using System;
using System.IO;
using System.Threading.Tasks;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Application.Services;

public class DosyaServisi : IDosyaServisi
{
    private readonly IWebHostEnvironment _env;

    public DosyaServisi(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> GorselYukleAsync(IFormFile dosya, string altKlasor)
    {
        if (dosya == null || dosya.Length == 0)
        {
            throw new ArgumentException("Yüklenmek istenen dosya boş veya geçersiz.");
        }

        // WebAPI projesindeki wwwroot/images/altKlasor yolunu fiziksel olarak belirliyoruz
        string uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "images", altKlasor);

        // Eğer klasör hiyerarşisi yoksa, otomatik olarak oluştururuz
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        // Aynı isimde dosyaların çakışmaması için benzersiz bir isim (GUID) üretiyoruz
        string uzanti = Path.GetExtension(dosya.FileName);
        string benzersizDosyaAdi = Guid.NewGuid().ToString("N") + uzanti;
        string fizikselDosyaYolu = Path.Combine(uploadsFolder, benzersizDosyaAdi);

        // Dosyayı sunucuya kopyalıyoruz
        using (var fileStream = new FileStream(fizikselDosyaYolu, FileMode.Create))
        {
            await dosya.CopyToAsync(fileStream);
        }

        // Veritabanına kaydedilecek olan ve tarayıcıdan erişilebilen göreceli (relative) URL'i dönüyoruz
        return $"/images/{altKlasor}/{benzersizDosyaAdi}";
    }

    public void GorselSil(string gorselUrl)
    {
        if (string.IsNullOrEmpty(gorselUrl)) return;

        // URL formatını (örn: /images/urunler/abc.jpg) fiziksel dosya yoluna çeviriyoruz
        string fizikselYol = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), gorselUrl.TrimStart('/'));

        if (File.Exists(fizikselYol))
        {
            File.Delete(fizikselYol);
        }
    }
}