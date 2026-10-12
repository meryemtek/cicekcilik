namespace ECommerce.Application.DTOs.Urun;

public class UrunEkleDto
{
    public string Ad { get; set; } = string.Empty;
    public string SeoUrl { get; set; } = string.Empty;
    public string? UrunKodu { get; set; }
    public string Aciklama { get; set; } = string.Empty;
    public string AiAciklama { get; set; } = string.Empty;
    public decimal Fiyat { get; set; }
    public int StokMiktari { get; set; }
    public string GorselUrl { get; set; } = string.Empty;
    public string? Etiket { get; set; }
    public Guid KategoriId { get; set; }
    public bool AktifMi { get; set; } = true;
}