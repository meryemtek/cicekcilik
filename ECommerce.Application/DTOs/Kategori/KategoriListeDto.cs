namespace ECommerce.Application.DTOs.Kategori;

public class KategoriListeDto
{
    public Guid Id { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string SeoUrl { get; set; } = string.Empty;
    public string GorselUrl { get; set; } = string.Empty;
    public bool AktifMi { get; set; }
    public Guid? UstKategoriId { get; set; }
    public string? UstKategoriAdi { get; set; }
}