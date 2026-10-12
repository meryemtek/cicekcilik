namespace ECommerce.Application.DTOs.KartNotuSablonu;

public class KartNotuSablonuListeDto
{
    public Guid Id { get; set; }
    public string Kategori { get; set; } = string.Empty;
    public string MesajIcerigi { get; set; } = string.Empty;
    public int SiraNo { get; set; }
    public bool AktifMi { get; set; }
}
