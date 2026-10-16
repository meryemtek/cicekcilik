namespace ECommerce.Domain.Entities;

public class Urun : TemelVarlik
{
    public string Ad { get; set; } = string.Empty;
    public string? UrunKodu { get; set; }
    public string SeoUrl { get; set; } = string.Empty;
    public string Aciklama { get; set; } = string.Empty;
    public string AiAciklama { get; set; } = string.Empty;
    public decimal Fiyat { get; set; }
    public int StokMiktari { get; set; }
    public string GorselUrl { get; set; } = string.Empty;
    public string? EkGorsellerJson { get; set; }
    public bool AktifMi { get; set; } = true;
    public string? Etiket { get; set; }
    public Guid KategoriId { get; set; }
    public virtual Kategori Kategori { get; set; } = null!;

    public virtual ICollection<SiparisKalemi> SiparisKalemleri { get; set; } = new List<SiparisKalemi>();
    public virtual ICollection<UrunVaryant> Varyantlar { get; set; } = new List<UrunVaryant>();
    public virtual ICollection<UrunKategori> UrunKategorileri { get; set; } = new List<UrunKategori>();
}