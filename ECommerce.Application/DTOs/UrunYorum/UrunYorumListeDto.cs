namespace ECommerce.Application.DTOs.UrunYorum;

public class UrunYorumListeDto
{
    public Guid Id { get; set; }
    public Guid UrunId { get; set; }
    public string YorumcuAd { get; set; } = string.Empty;
    public string YorumMetni { get; set; } = string.Empty;

    public byte Puan { get; set; }
    public bool OnaylandiMi { get; set; }
    public DateTime OlusturulmaTarihi { get; set; }
}