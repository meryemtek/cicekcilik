using ECommerce.Application.DTOs.OneCikan;
using ECommerce.Application.DTOs.Urun;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services
{
    public class OneCikanServisi : IOneCikanServisi
    {
        private readonly IDepo<OneCikanAyar> _ayarDeposu;
        private readonly IDepo<Urun> _urunDeposu;

        public OneCikanServisi(IDepo<OneCikanAyar> ayarDeposu, IDepo<Urun> urunDeposu)
        {
            _ayarDeposu = ayarDeposu;
            _urunDeposu = urunDeposu;
        }

        public async Task<OneCikanDto> AyarlariGetirAsync()
        {
            var ayarlar = (await _ayarDeposu.TumunuGetirAsync()).FirstOrDefault();
            if (ayarlar == null)
            {
                return new OneCikanDto { TumUrunleriGoster = true };
            }

            var kategoriIdleri = string.IsNullOrEmpty(ayarlar.SecilenKategoriIdleri)
                ? new List<Guid>()
                : ayarlar.SecilenKategoriIdleri.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Guid.Parse).ToList();

            var urunIdleri = string.IsNullOrEmpty(ayarlar.SecilenUrunIdleri)
                ? new List<Guid>()
                : ayarlar.SecilenUrunIdleri.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Guid.Parse).ToList();

            return new OneCikanDto
            {
                TumUrunleriGoster = ayarlar.TumUrunleriGoster,
                SecilenKategoriIdleri = kategoriIdleri,
                SecilenUrunIdleri = urunIdleri
            };
        }

        public async Task AyarlariGuncelleAsync(OneCikanDto dto)
        {
            var tumAyarlar = await _ayarDeposu.TumunuGetirAsync();
            var ayarlar = tumAyarlar.FirstOrDefault();

            var kategoriIdleriString = dto.SecilenKategoriIdleri != null && dto.SecilenKategoriIdleri.Any()
                ? string.Join(",", dto.SecilenKategoriIdleri)
                : string.Empty;

            var urunIdleriString = dto.SecilenUrunIdleri != null && dto.SecilenUrunIdleri.Any()
                ? string.Join(",", dto.SecilenUrunIdleri)
                : string.Empty;

            if (ayarlar == null)
            {
                ayarlar = new OneCikanAyar
                {
                    TumUrunleriGoster = dto.TumUrunleriGoster,
                    SecilenKategoriIdleri = kategoriIdleriString,
                    SecilenUrunIdleri = urunIdleriString,
                    OlusturulmaTarihi = DateTime.UtcNow
                };
                await _ayarDeposu.EkleAsync(ayarlar);
            }
            else
            {
                ayarlar.TumUrunleriGoster = dto.TumUrunleriGoster;
                ayarlar.SecilenKategoriIdleri = kategoriIdleriString;
                ayarlar.SecilenUrunIdleri = urunIdleriString;
                ayarlar.GuncellenmeTarihi = DateTime.UtcNow;
                _ayarDeposu.Guncelle(ayarlar);
            }

            await _ayarDeposu.DegisiklikleriKaydetAsync();
        }

        public async Task<List<UrunListeDto>> OneCikanUrunleriGetirAsync()
        {
            var ayarlar = await AyarlariGetirAsync();
            var tumUrunler = await _urunDeposu.TumunuGetirAsync();

            var aktifUrunler = tumUrunler.Where(x => x.AktifMi);

            // Eğer tüm ürünleri göster kapalıysa, hem ürün hem kategori filtrelemesini uygulayabiliriz
            if (!ayarlar.TumUrunleriGoster)
            {
                aktifUrunler = aktifUrunler.Where(x =>
                    (ayarlar.SecilenUrunIdleri != null && ayarlar.SecilenUrunIdleri.Contains(x.Id)) ||
                    (ayarlar.SecilenKategoriIdleri != null && ayarlar.SecilenKategoriIdleri.Contains(x.KategoriId))
                );
            }

            return aktifUrunler.Select(u => new UrunListeDto
            {
                Id = u.Id,
                Ad = u.Ad,
                SeoUrl = u.SeoUrl,
                UrunKodu = u.UrunKodu,
                Fiyat = u.Fiyat,
                StokMiktari = u.StokMiktari,
                AktifMi = u.AktifMi,
                KategoriId = u.KategoriId,
                Etiket = u.Etiket,
                GorselUrl = u.GorselUrl
            }).ToList();
        }
    }
}