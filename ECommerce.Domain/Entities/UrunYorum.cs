namespace ECommerce.Domain.Entities;

public class UrunYorum : TemelVarlik
{
    public Guid UrunId { get; set; }
    public virtual Urun Urun { get; set; } = null!;

    public string YorumcuAd { get; set; } = string.Empty;
    public string YorumMetni { get; set; } = string.Empty;
    public byte Puan { get; set; } // 1 ile 5 arası yıldız
    public bool OnaylandiMi { get; set; } = false; // Admin onayından geçmeden sitede görünmez
}