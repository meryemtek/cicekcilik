using System;

namespace ECommerce.Domain.Entities;

public class UrunKategori : TemelVarlik
{
    public Guid UrunId { get; set; }
    public virtual Urun Urun { get; set; } = null!;

    public Guid KategoriId { get; set; }
    public virtual Kategori Kategori { get; set; } = null!;
}
