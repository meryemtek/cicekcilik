namespace ECommerce.Application.DTOs.Blog;

public class BlogEkleDto
{
    public string Baslik { get; set; } = string.Empty;
    public string SeoUrl { get; set; } = string.Empty;
    public string? MetaBaslik { get; set; }
    public string? MetaAciklama { get; set; }
    public string Ozet { get; set; } = string.Empty;
    public string Icerik { get; set; } = string.Empty;
    public string GorselUrl { get; set; } = string.Empty;
    public bool AktifMi { get; set; } = true;
    public DateTime YayinlanmaTarihi { get; set; } = DateTime.UtcNow;
}
