using ECommerce.Application.DTOs.Favori;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;

public class FavoriServisi : IFavoriServisi
{
    private readonly IDepo<Favori> _favoriDeposu;
    private readonly IDepo<Urun> _urunDeposu;

    public FavoriServisi(IDepo<Favori> favoriDeposu, IDepo<Urun> urunDeposu)
    {
        _favoriDeposu = favoriDeposu;
        _urunDeposu = urunDeposu;
    }

    public async Task<IReadOnlyList<FavoriListeDto>> KullaniciFavorileriniGetirAsync(string kullaniciId)
    {
        var favoriler = await _favoriDeposu.GetirAsync(f => f.KullaniciId == kullaniciId);
        var sonuc = new List<FavoriListeDto>();

        foreach (var fav in favoriler)
        {
            var urun = await _urunDeposu.IdIleGetirAsync(fav.UrunId);
            if (urun != null)
            {
                sonuc.Add(new FavoriListeDto
                {
                    Id = fav.Id,
                    UrunId = urun.Id,
                    UrunAdi = urun.Ad,
                    SeoUrl = urun.SeoUrl,
                    GorselUrl = urun.GorselUrl,
                    Fiyat = urun.Fiyat,
                    Etiket = urun.Etiket
                });
            }
        }

        return sonuc;
    }

    public async Task<bool> FavoriToggleAsync(string kullaniciId, Guid urunId)
    {
        var mevcutListesi = await _favoriDeposu.GetirAsync(f => f.KullaniciId == kullaniciId && f.UrunId == urunId);
        var mevcut = mevcutListesi.FirstOrDefault();

        if (mevcut != null)
        {
            // Zaten favorilerdeyse çıkar (Sil)
            _favoriDeposu.Sil(mevcut);
            await _favoriDeposu.DegisiklikleriKaydetAsync();
            return false; // Artık favoride değil
        }
        else
        {
            // Favorilerde yoksa ekle
            var yeniFavori = new Favori
            {
                KullaniciId = kullaniciId,
                UrunId = urunId
            };
            await _favoriDeposu.EkleAsync(yeniFavori);
            await _favoriDeposu.DegisiklikleriKaydetAsync();
            return true; // Favorilere eklendi
        }
    }

    public async Task<bool> FavorideMiAsync(string kullaniciId, Guid urunId)
    {
        var mevcutListesi = await _favoriDeposu.GetirAsync(f => f.KullaniciId == kullaniciId && f.UrunId == urunId);
        return mevcutListesi.Any();
    }
}