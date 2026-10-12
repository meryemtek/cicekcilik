namespace ECommerce.Domain.Entities;

public class Slider : TemelVarlik
{
    public Guid Id { get; set; }
    public string? Baslik { get; set; }         // Örn: Sevgililer Gününe Özel
    public string? AltMetin { get; set; }       // Örn: Seçili orkidelerde %20 indirim
    public string ResimUrl { get; set; }       // Yüklenen görselin yolu
    public string? LinkUrl { get; set; }        // Yönlendirilecek URL
    public string? ButonMetni { get; set; }     // Örn: Fırsatları Gör
    public int SiraNo { get; set; }
    public bool AktifMi { get; set; } = true;
}