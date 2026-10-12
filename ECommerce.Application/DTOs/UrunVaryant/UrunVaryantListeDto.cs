namespace ECommerce.Application.DTOs.UrunVaryant;

public class UrunVaryantListeDto
{
    public Guid Id { get; set; }
    public Guid UrunId { get; set; }
    public string VaryantAdi { get; set; } = string.Empty;
    public decimal FiyatFarki { get; set; }
    public int StokMiktari { get; set; }
    public bool AktifMi { get; set; }
}