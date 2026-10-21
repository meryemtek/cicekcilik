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
        var fileUrl = await DosyaYukleAsync(dosya, Path.Combine("images", altKlasor));
        return fileUrl.Replace('\\', '/');
    }

    public async Task<string> DosyaYukleAsync(IFormFile dosya, string altKlasor)
    {
        if (dosya == null || dosya.Length == 0)
        {
            throw new ArgumentException("Yüklenmek istenen dosya boş veya geçersiz.");
        }

        if (string.IsNullOrWhiteSpace(altKlasor) || Path.IsPathRooted(altKlasor) || altKlasor.Contains(".."))
        {
            throw new ArgumentException("Dosya klasörü geçersiz.", nameof(altKlasor));
        }

        string uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", altKlasor);

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        string uzanti = Path.GetExtension(Path.GetFileName(dosya.FileName));
        string benzersizDosyaAdi = Guid.NewGuid().ToString("N") + uzanti;
        string fizikselDosyaYolu = Path.Combine(uploadsFolder, benzersizDosyaAdi);

        using (var fileStream = new FileStream(fizikselDosyaYolu, FileMode.Create))
        {
            await dosya.CopyToAsync(fileStream);
        }

        return $"/uploads/{altKlasor.Replace('\\', '/')}/{benzersizDosyaAdi}";
    }

    public async Task<(Stream Stream, string ContentType, string FileName)?> DosyaOkuAsync(string dosyaYolu)
    {
        if (string.IsNullOrWhiteSpace(dosyaYolu) || Uri.TryCreate(dosyaYolu, UriKind.Absolute, out _))
        {
            return null;
        }

        var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var normalizedRoot = Path.GetFullPath(webRootPath);
        var filePath = Path.GetFullPath(Path.Combine(normalizedRoot, dosyaYolu.TrimStart('/', '\\')));
        if (!filePath.StartsWith(normalizedRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) || !File.Exists(filePath))
        {
            return null;
        }

        var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(filePath, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        return (new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read), contentType, Path.GetFileName(filePath));
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
