using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace ECommerce.WebUI.Services;

public static class ApiGorselYukleyici
{
    public static async Task<string> YukleAsync(
        IHttpClientFactory httpClientFactory,
        ClaimsPrincipal user,
        IFormFile dosya,
        string altKlasor)
    {
        var client = httpClientFactory.CreateClient("ApiClient");
        var token = user.FindFirst("JWToken")?.Value;
        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        using var content = new MultipartFormDataContent();
        await using var stream = dosya.OpenReadStream();
        using var fileContent = new StreamContent(stream);
        if (!string.IsNullOrWhiteSpace(dosya.ContentType))
        {
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(dosya.ContentType);
        }

        content.Add(fileContent, "dosya", Path.GetFileName(dosya.FileName));
        content.Add(new StringContent(altKlasor), "altKlasor");

        using var response = await client.PostAsync("admin/dosyalar/gorsel-yukle", content);
        var responseBody = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Görsel kalıcı depolamaya yüklenemedi.");
        }

        using var document = JsonDocument.Parse(responseBody);
        foreach (var property in document.RootElement.EnumerateObject())
        {
            if (property.Name.Equals("Url", StringComparison.OrdinalIgnoreCase) &&
                property.Value.ValueKind == JsonValueKind.String)
            {
                var url = property.Value.GetString();
                if (!string.IsNullOrWhiteSpace(url))
                {
                    if (Uri.TryCreate(url, UriKind.Absolute, out _))
                    {
                        return url;
                    }

                    var apiRoot = client.BaseAddress?.GetLeftPart(UriPartial.Authority)
                        ?? throw new InvalidOperationException("API adresi tanımlı değil.");
                    return $"{apiRoot}/{url.TrimStart('/')}";
                }
            }
        }

        throw new InvalidOperationException("Görsel yüklemesinden geçerli adres alınamadı.");
    }
}
