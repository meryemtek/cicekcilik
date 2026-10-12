namespace ECommerce.Application.DTOs.Slider;

public class SliderGuncelleDto
{
    public Guid Id { get; set; }
    public string? Baslik { get; set; }
    public string? AltMetin { get; set; }
    public string ResimUrl { get; set; }
    public string? LinkUrl { get; set; }
    public string? ButonMetni { get; set; }
    public int SiraNo { get; set; }
    public bool AktifMi { get; set; }
}