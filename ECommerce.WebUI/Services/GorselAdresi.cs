namespace ECommerce.WebUI.Services;

public static class GorselAdresi
{
    public static string GoruntulenebilirYap(string? adres, Uri? apiBaseAddress)
    {
        if (string.IsNullOrWhiteSpace(adres) || apiBaseAddress is null ||
            !Uri.TryCreate(adres, UriKind.Absolute, out var uri) ||
            !uri.Host.EndsWith(".neon.tech", StringComparison.OrdinalIgnoreCase))
        {
            return adres ?? string.Empty;
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
