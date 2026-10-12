namespace ECommerce.Application.DTOs.SemtTeslimatBolgesi;

public class SemtBolgeListeDto
{
    public Guid Id { get; set; }
    public string IlAdi { get; set; } = string.Empty;
    public string IlceAdi { get; set; } = string.Empty;
    public string MahalleAdi { get; set; } = string.Empty;
    public decimal EkTeslimatUcreti { get; set; }
    public bool AktifMi { get; set; }
}