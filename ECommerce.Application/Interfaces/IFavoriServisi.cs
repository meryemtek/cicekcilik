using ECommerce.Application.DTOs.Favori;

public interface IFavoriServisi
{
    Task<IReadOnlyList<FavoriListeDto>> KullaniciFavorileriniGetirAsync(string kullaniciId);
    Task<bool> FavoriToggleAsync(string kullaniciId, Guid urunId); // Ekliyse çıkar, değilse ekle
    Task<bool> FavorideMiAsync(string kullaniciId, Guid urunId);
}