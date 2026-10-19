using ECommerce.Domain.Entities;

public class MenuGrup : TemelVarlik
{
    public Guid Id { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string? Aciklama { get; set; }
    public string Konum { get; set; } = string.Empty;
    public bool AktifMi { get; set; }
    public bool SistemMi { get; set; } // Arayüzde sil butonunu gizlemek için
    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}
