namespace ECommerce.Application.DTOs.Kupon;

public class KuponGuncelleDto
{
    public Guid Id { get; set; }
    public string Kod { get; set; } = string.Empty;
    public decimal IndirimMiktari { get; set; }
    public bool YuzdelikMi { get; set; }
    public DateTime BitisTarihi { get; set; }
    public bool AktifMi { get; set; }
}