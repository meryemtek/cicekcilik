namespace ECommerce.Application.DTOs.UrunVaryant;

public class UrunVaryantEkleDto
{
    public Guid UrunId { get; set; }
    public string VaryantAdi { get; set; } = string.Empty;
    public decimal FiyatFarki { get; set; }
    public int StokMiktari { get; set; }
    public bool AktifMi { get; set; } = true;
}