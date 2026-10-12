namespace ECommerce.Application.DTOs.UrunYorum;

public class UrunYorumEkleDto
{
    public Guid UrunId { get; set; }
    public string YorumcuAd { get; set; } = string.Empty;
    public string YorumMetni { get; set; } = string.Empty;
    public byte Puan { get; set; }
}