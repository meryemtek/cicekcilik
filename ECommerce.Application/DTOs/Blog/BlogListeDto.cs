namespace ECommerce.Application.DTOs.Blog;

public class BlogListeDto
{
    public Guid Id { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string SeoUrl { get; set; } = string.Empty;
    public string Ozet { get; set; } = string.Empty;
    public string GorselUrl { get; set; } = string.Empty;
    public int OkunmaSayisi { get; set; }
    public bool AktifMi { get; set; }
    public DateTime YayinlanmaTarihi { get; set; }
}


