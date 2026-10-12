using System;

namespace ECommerce.Application.DTOs.Kategori;

public class KategoriGuncelleDto
{
    public Guid Id { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string SeoUrl { get; set; } = string.Empty;
    public string? Aciklama { get; set; }
    public string? GorselUrl { get; set; }
    public bool AktifMi { get; set; }
    public Guid? UstKategoriId { get; set; }
}