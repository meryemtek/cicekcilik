namespace ECommerce.WebUI.Models;

public class LoginViewModel
{
    public string Email { get; set; } = string.Empty;
    public string Sifre { get; set; } = string.Empty;
    public bool BeniHatirla { get; set; }

    // API'den dinamik olarak çekip Ekrana basacağımız alanlar
    public string FirmaAdi { get; set; } = "Yönetim Paneli";
    public string LogoUrl { get; set; } = string.Empty;
}

// API'den dönen Token yanıtını yakalamak için
public class TokenResponse
{
    public string Token { get; set; } = string.Empty;
}

// API'den dönen Site Ayarları özetini yakalamak için
public class SiteAyariOzetResponse
{
    public string FirmaAdi { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string TemaRengiHex { get; set; } = string.Empty;
}