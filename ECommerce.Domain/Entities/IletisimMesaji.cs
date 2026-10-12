namespace ECommerce.Domain.Entities;

public class IletisimMesaji : TemelVarlik
{
    public string GonderenAdSoyad { get; set; } = string.Empty;
    public string GonderenEmail { get; set; } = string.Empty;
    public string Telefon { get; set; } = string.Empty;
    public string Konu { get; set; } = string.Empty;
    public string Mesaj { get; set; } = string.Empty;
    public bool OkunduMu { get; set; } = false;
}