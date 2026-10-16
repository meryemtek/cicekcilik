namespace ECommerce.Application.DTOs.Urun;

public class UrunListeDto
{
    public Guid Id { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string? UrunKodu { get; set; }
    public string SeoUrl { get; set; } = string.Empty;
    public decimal Fiyat { get; set; }
    public int StokMiktari { get; set; }
    public bool AktifMi { get; set; }
    public Guid KategoriId { get; set; }
    public List<Guid> AltKategoriIdleri { get; set; } = new();
    public string GorselUrl { get; set; } = string.Empty;
    public string? Etiket { get; set; }
}