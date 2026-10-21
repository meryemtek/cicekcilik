using Amazon.S3;
using Amazon.S3.Model;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Services;

public sealed class NeonObjectStorageDosyaServisi : IDosyaServisi
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _publicBucket;
    private readonly string _privateBucket;

    public NeonObjectStorageDosyaServisi(IConfiguration configuration, IAmazonS3 s3Client)
    {
        _publicBucket = configuration["NEON_STORAGE_BUCKET"]
            ?? throw new InvalidOperationException("NEON_STORAGE_BUCKET ayarı eksik.");
        _privateBucket = configuration["NEON_PRIVATE_STORAGE_BUCKET"]
            ?? throw new InvalidOperationException("NEON_PRIVATE_STORAGE_BUCKET ayarı eksik.");
        _s3Client = s3Client;
    }

    public async Task<string> GorselYukleAsync(IFormFile dosya, string altKlasor)
    {
        if (dosya == null || dosya.Length == 0)
        {
            throw new ArgumentException("Yüklenmek istenen dosya boş veya geçersiz.");
        }

        if (string.IsNullOrWhiteSpace(altKlasor) || altKlasor.Any(character =>
                !char.IsAsciiLetterOrDigit(character) && character != '-' && character != '_'))
        {
            throw new ArgumentException("Görsel klasör adı geçersiz.", nameof(altKlasor));
        }

        return await YukleAsync(dosya, $"images/{altKlasor}", _publicBucket, privateReference: false);
    }

    public Task<string> DosyaYukleAsync(IFormFile dosya, string altKlasor)
    {
        return YukleAsync(dosya, altKlasor, _privateBucket, privateReference: true);
    }

    private async Task<string> YukleAsync(IFormFile dosya, string altKlasor, string bucket, bool privateReference)
    {
        if (dosya == null || dosya.Length == 0)
        {
            throw new ArgumentException("Yüklenmek istenen dosya boş veya geçersiz.");
        }

        if (string.IsNullOrWhiteSpace(altKlasor) || altKlasor.Split('/').Any(segment =>
                string.IsNullOrWhiteSpace(segment) || segment.Any(character =>
                    !char.IsAsciiLetterOrDigit(character) && character != '-' && character != '_')))
        {
            throw new ArgumentException("Görsel klasör adı geçersiz.", nameof(altKlasor));
        }

        var extension = Path.GetExtension(Path.GetFileName(dosya.FileName));
        var key = $"{altKlasor}/{Guid.NewGuid():N}{extension}";

        await using var stream = dosya.OpenReadStream();
        await _s3Client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = bucket,
            Key = key,
            InputStream = stream,
            ContentType = string.IsNullOrWhiteSpace(dosya.ContentType)
                ? "application/octet-stream"
                : dosya.ContentType
        });

        if (privateReference)
        {
            return $"neon-private://{Uri.EscapeDataString(key).Replace("%2F", "/", StringComparison.OrdinalIgnoreCase)}";
        }

        return $"/api/dosyalar/gorseller/{key}";
    }

    public async Task<(Stream Stream, string ContentType, string FileName)?> DosyaOkuAsync(string dosyaYolu)
    {
        const string privatePrefix = "neon-private://";
        if (!dosyaYolu.StartsWith(privatePrefix, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var key = Uri.UnescapeDataString(dosyaYolu[privatePrefix.Length..]);
        using var response = await _s3Client.GetObjectAsync(_privateBucket, key);
        var stream = new MemoryStream();
        await response.ResponseStream.CopyToAsync(stream);
        stream.Position = 0;
        return (stream, response.Headers.ContentType ?? "application/octet-stream", Path.GetFileName(key));
    }

    public async Task<(Stream Stream, string ContentType)?> GorselOkuAsync(string anahtar)
    {
        if (!anahtar.StartsWith("images/", StringComparison.Ordinal) ||
            anahtar.Split('/').Any(parca => string.IsNullOrWhiteSpace(parca) || parca is "." or ".."))
        {
            return null;
        }

        try
        {
            using var response = await _s3Client.GetObjectAsync(_publicBucket, anahtar);
            var stream = new MemoryStream();
            await response.ResponseStream.CopyToAsync(stream);
            stream.Position = 0;
            return (stream, response.Headers.ContentType ?? "application/octet-stream");
        }
        catch (Amazon.S3.AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public void GorselSil(string gorselUrl)
    {
        if (!Uri.TryCreate(gorselUrl, UriKind.Absolute, out var uri))
        {
            return;
        }

        var path = Uri.UnescapeDataString(uri.AbsolutePath);
        var prefix = path.StartsWith($"/{_publicBucket}/", StringComparison.Ordinal)
            ? $"/{_publicBucket}/"
            : "/api/dosyalar/gorseller/";
        if (!path.StartsWith(prefix, StringComparison.Ordinal))
        {
            return;
        }

        var key = path[prefix.Length..];
        if (!string.IsNullOrWhiteSpace(key))
        {
            _s3Client.DeleteObjectAsync(_publicBucket, key).GetAwaiter().GetResult();
        }
    }
}
