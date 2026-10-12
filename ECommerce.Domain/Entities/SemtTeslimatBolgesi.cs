namespace ECommerce.Domain.Entities;

public class SemtTeslimatBolgesi : TemelVarlik
{
    public string IlAdi { get; set; } = string.Empty;
    public string IlceAdi { get; set; } = string.Empty;
    public string MahalleAdi { get; set; } = string.Empty;
    public decimal EkTeslimatUcreti { get; set; }
    public bool AktifMi { get; set; } = true;
}