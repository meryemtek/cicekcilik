namespace ECommerce.Application.DTOs.IletisimMesaji;

public class IletisimMesajiListeDto
{
    public Guid Id { get; set; }
    public string GonderenAdSoyad { get; set; } = string.Empty;
    public string GonderenEmail { get; set; } = string.Empty;
    public string Konu { get; set; } = string.Empty;
    public DateTime OlusturulmaTarihi { get; set; }
    public bool OkunduMu { get; set; }
}