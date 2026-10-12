namespace ECommerce.Domain.Entities;

public class KartNotuSablonu : TemelVarlik
{
    public string Kategori { get; set; } = string.Empty; // Doğum Günü, Yıldönümü vb.
    public string MesajIcerigi { get; set; } = string.Empty;
    public int SiraNo { get; set; }
    public bool AktifMi { get; set; } = true;
}