namespace ECommerce.Application.DTOs.SSS;

public class SssListeDto
{
    public Guid Id { get; set; }
    public string Soru { get; set; } = string.Empty;
    public int SiraNo { get; set; }
    public bool AktifMi { get; set; }
}