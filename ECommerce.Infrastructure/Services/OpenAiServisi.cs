using ECommerce.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Services;

public class OpenAiServisi : IAiServisi
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public OpenAiServisi(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<string> UrunAciklamasiUretAsync(string urunAdi, string kategoriAdi)
    {
        var apiKey = _configuration["OpenAI:ApiKey"];

        // Eğer API key girilmemişse veya hala varsayılan metindeyse test amaçlı şık bir metin döner
        if (string.IsNullOrEmpty(apiKey) || apiKey.Contains("senin-openai"))
        {
            await Task.Delay(400);
            return $"<p><strong>{urunAdi}</strong> ({kategoriAdi}), en taze ve kaliteli malzemelerle özenle tasarlanmış seçkin bir aranjmandır. Sevdiklerinizi mutlu etmek için kusursuz bir tercihtir.</p>";
        }

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var prompt = $"E-ticaret mağazamızdaki '{kategoriAdi}' kategorisinde yer alan '{urunAdi}' isimli ürün için HTML formatında (<p> ve <strong> etiketleri kullanarak), SEO uyumlu, dönüşüm odaklı ve akıcı bir ürün açıklaması yazar mısın? Sadece HTML içeriğini döndür, başka açıklama ekleme.";

        var payload = new
        {
            model = "gpt-4o-mini",
            messages = new[]
            {
                new { role = "system", content = "Sen profesyonel bir e-ticaret içerik yazarı ve SEO uzmanısın." },
                new { role = "user", content = prompt }
            },
            temperature = 0.7
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);

        if (!response.IsSuccessStatusCode)
        {
            return $"<p><strong>{urunAdi}</strong>, zarif tasarımıyla öne çıkan özel bir üründür.</p>";
        }

        var responseString = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseString);
        var root = doc.RootElement;
        var choice = root.GetProperty("choices")[0];
        var message = choice.GetProperty("message");
        var text = message.GetProperty("content").GetString();

        return text?.Trim() ?? $"<p>{urunAdi}</p>";
    }
}