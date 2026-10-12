namespace ECommerce.Domain.Entities;

public class UrunVaryant : TemelVarlik
{
    public Guid UrunId { get; set; }
    public virtual Urun Urun { get; set; } = null!;
    public string VaryantAdi { get; set; } = string.Empty; // Örn: Büyük Boy (Lux), Çelenk Yazılı
    public decimal FiyatFarki { get; set; }
    public int StokMiktari { get; set; }
    public bool AktifMi { get; set; } = true;
}