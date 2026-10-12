namespace ECommerce.Domain.Entities;

public class BlogYazisi : TemelVarlik
{
    public string Baslik { get; set; } = string.Empty;
    public string SeoUrl { get; set; } = string.Empty;

    // SEO için kritik alanlar
    public string? MetaBaslik { get; set; }
    public string? MetaAciklama { get; set; }

    public string Ozet { get; set; } = string.Empty;
    public string Icerik { get; set; } = string.Empty; // Zengin içerik (HTML/Görsel) buraya gelecek
    public string GorselUrl { get; set; } = string.Empty;

    public int OkunmaSayisi { get; set; } = 0;
    public bool AktifMi { get; set; } = true;

    // Yayınlanma tarihi, sıralama vb. için
    public DateTime YayinlanmaTarihi { get; set; } = DateTime.UtcNow;
}