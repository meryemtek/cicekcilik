namespace ECommerce.Domain.Entities;

public class SiparisGorselOnay : TemelVarlik
{
    public Guid SiparisId { get; set; }
    public virtual Siparis Siparis { get; set; } = null!;
    public string GorselUrl { get; set; } = string.Empty;
    public DateTime YuklemeTarihi { get; set; } = DateTime.UtcNow;
    public bool MusteriOnayladiMi { get; set; } = false;
}