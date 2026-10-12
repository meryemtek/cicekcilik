using System;
using System.Collections.Generic;

namespace ECommerce.Domain.Entities;

public class Kategori : TemelVarlik
{
    public string Ad { get; set; } = string.Empty;
    public string SeoUrl { get; set; } = string.Empty;
    public string Aciklama { get; set; } = string.Empty;
    public string GorselUrl { get; set; } = string.Empty;
    public bool AktifMi { get; set; } = true;

    // --- HİYERARŞİ (ALT KATEGORİ) ALANLARI ---
    public Guid? UstKategoriId { get; set; }
    public virtual Kategori? UstKategori { get; set; }
    public virtual ICollection<Kategori> AltKategoriler { get; set; } = new List<Kategori>();
    // ----------------------------------------

    public virtual ICollection<Urun> Urunler { get; set; } = new List<Urun>();
}