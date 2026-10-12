namespace ECommerce.Application.DTOs.Kupon;

public class KuponEkleDto
{
    public string Kod { get; set; } = string.Empty;
    public decimal IndirimMiktari { get; set; }
    public bool YuzdelikMi { get; set; }
    public DateTime BitisTarihi { get; set; }
    public bool AktifMi { get; set; } = true;
}