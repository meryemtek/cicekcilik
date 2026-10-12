using ECommerce.Domain.Entities;

public class Favori : TemelVarlik
{
    public string KullaniciId { get; set; } = string.Empty; // Giriş yapmış kullanıcı için
    public Guid UrunId { get; set; }
    public virtual Urun Urun { get; set; } = null!;
}