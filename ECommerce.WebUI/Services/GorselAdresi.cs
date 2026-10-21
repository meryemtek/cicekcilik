namespace ECommerce.WebUI.Services;

public static class GorselAdresi
{
    public static string GoruntulenebilirYap(string? adres, Uri? apiBaseAddress)
    {
        if (string.IsNullOrWhiteSpace(adres) || apiBaseAddress is null)
        {
            return adres ?? string.Empty;
        }

        if (adres.StartsWith("/api/dosyalar/gorseller/", StringComparison.Ordinal))
        {
            return new Uri(new Uri(apiBaseAddress.GetLeftPart(UriPartial.Authority)), adres).ToString();
        }

        if (!Uri.TryCreate(adres, UriKind.Absolute, out var uri) ||
            !uri.Host.EndsWith(".neon.tech", StringComparison.OrdinalIgnoreCase))
        {
            return adres;
        }

        var path = uri.AbsolutePath;
        var imagesIndex = path.IndexOf("/images/", StringComparison.Ordinal);
        if (imagesIndex < 0)
        {
            return adres;
        }

        var anahtar = path[(imagesIndex + 1)..];
        return new Uri(apiBaseAddress, $"dosyalar/gorseller/{anahtar}").ToString();
    }
}
