namespace ECommerce.Application.DTOs.Odeme;

public class OdemeYanitDto
{
    public bool BasariliMi { get; set; }
    public string TokenVeyaUrl { get; set; } = string.Empty; // PayTR iframe tokeni veya yönlendirme URL'si
    public string HataMesaji { get; set; } = string.Empty;
    public bool DemoModu { get; set; }
}
