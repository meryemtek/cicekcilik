namespace ECommerce.Application.DTOs.BankaHesabi;

public class BankaHesabiListeDto
{
    public Guid Id { get; set; }
    public string BankaAdi { get; set; } = string.Empty;
    public string Iban { get; set; } = string.Empty;
    public bool AktifMi { get; set; }
}