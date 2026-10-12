namespace ECommerce.Application.DTOs.SSS;

public class SssGuncelleDto
{
    public Guid Id { get; set; }
    public string Soru { get; set; } = string.Empty;
    public string Cevap { get; set; } = string.Empty;
    public int SiraNo { get; set; }
    public bool AktifMi { get; set; }
}