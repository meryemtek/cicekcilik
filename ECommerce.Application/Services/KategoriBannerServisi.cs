using ECommerce.Application.DTOs.KategoriBanner;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services
{
    public class KategoriBannerServisi : IKategoriBannerServisi
    {
        private readonly IDepo<KategoriBanner> _depo;
        private readonly IDepo<Kategori> _kategoriDeposu;

        public KategoriBannerServisi(IDepo<KategoriBanner> depo, IDepo<Kategori> kategoriDeposu)
        {
            _depo = depo;
            _kategoriDeposu = kategoriDeposu;
        }

        public async Task<List<KategoriBannerDto>> TumunuGetirAsync()
        {
            var liste = await _depo.TumunuGetirAsync();
            var kategoriler = await _kategoriDeposu.TumunuGetirAsync();

            return liste.OrderBy(x => x.Sira).Select(x => {
                var kat = kategoriler.FirstOrDefault(k => k.Id == x.KategoriId);
                return new KategoriBannerDto
                {
                    Id = x.Id,
                    Baslik = x.Baslik,
                    Aciklama = x.Aciklama,
                    GorselUrl = x.GorselUrl, // Görsel yolunun aktarıldığından emin olunuyor
                    KategoriId = x.KategoriId,
                    KategoriAdi = kat?.Ad,
                    KategoriSeoUrl = kat?.SeoUrl,
                    Sira = x.Sira,
                    AktifMi = x.AktifMi
                };
            }).ToList();
        }

        public async Task<KategoriBannerEkleGuncelleDto> GetirByIdAsync(Guid id)
        {
            var item = await _depo.IdIleGetirAsync(id);
            if (item == null) return new KategoriBannerEkleGuncelleDto();

            return new KategoriBannerEkleGuncelleDto
            {
                Id = item.Id,
                Baslik = item.Baslik,
                Aciklama = item.Aciklama,
                GorselUrl = item.GorselUrl, // İKİNCİ GÖRSEL SORUNU ÇÖZÜMÜ: GorselUrl eksiksiz mapleme yapıldı
                KategoriId = item.KategoriId,
                Sira = item.Sira,
                AktifMi = item.AktifMi
            };
        }

        public async Task EkleAsync(KategoriBannerEkleGuncelleDto dto)
        {
            var entity = new KategoriBanner
            {
                Baslik = dto.Baslik,
                Aciklama = dto.Aciklama,
                GorselUrl = dto.GorselUrl,
                KategoriId = dto.KategoriId,
                Sira = dto.Sira,
                AktifMi = dto.AktifMi,
                OlusturulmaTarihi = DateTime.UtcNow
            };
            await _depo.EkleAsync(entity);
            await _depo.DegisiklikleriKaydetAsync();
        }

        public async Task GuncelleAsync(KategoriBannerEkleGuncelleDto dto)
        {
            var entity = await _depo.IdIleGetirAsync(dto.Id);
            if (entity != null)
            {
                entity.Baslik = dto.Baslik;
                entity.Aciklama = dto.Aciklama;
                entity.KategoriId = dto.KategoriId;
                entity.Sira = dto.Sira;
                entity.AktifMi = dto.AktifMi;
                entity.GuncellenmeTarihi = DateTime.UtcNow;

                if (!string.IsNullOrWhiteSpace(dto.GorselUrl))
                {
                    entity.GorselUrl = dto.GorselUrl;
                }

                _depo.Guncelle(entity);
                await _depo.DegisiklikleriKaydetAsync();
            }
        }

        public async Task SilAsync(Guid id)
        {
            var entity = await _depo.IdIleGetirAsync(id);
            if (entity != null)
            {
                _depo.Sil(entity);
                await _depo.DegisiklikleriKaydetAsync();
            }
        }
    }
}