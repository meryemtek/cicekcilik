using ECommerce.Application.DTOs.AnasayfaKategori;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;

namespace ECommerce.Infrastructure.Services
{
    public class AnasayfaKategoriServisi : IAnasayfaKategoriServisi
    {
        private readonly IDepo<AnasayfaKategori> _anasayfaKategoriDeposu;
        private readonly IDepo<Kategori> _kategoriDeposu;

        public AnasayfaKategoriServisi(
            IDepo<AnasayfaKategori> anasayfaKategoriDeposu,
            IDepo<Kategori> kategoriDeposu)
        {
            _anasayfaKategoriDeposu = anasayfaKategoriDeposu;
            _kategoriDeposu = kategoriDeposu;
        }

        public async Task<IReadOnlyList<AnasayfaKategoriDto>> AktifleriGetirAsync()
        {
            var anasayfaKategorileri =
                await _anasayfaKategoriDeposu.GetirAsync(x => x.AktifMi);

            var kategoriler =
                await _kategoriDeposu.GetirAsync(x => x.AktifMi);

            return anasayfaKategorileri
                .OrderBy(x => x.SiraNo)
                .Join(
                    kategoriler,
                    ak => ak.KategoriId,
                    k => k.Id,
                    (ak, k) => new AnasayfaKategoriDto
                    {
                        Id = ak.Id,
                        KategoriId = k.Id,
                        KategoriAdi = k.Ad,
                        SeoUrl = k.SeoUrl,
                        GorselUrl = ak.GorselUrl ?? k.GorselUrl,
                        VitrinGorselUrl = ak.GorselUrl,
                        Aciklama = k.Aciklama,
                        SiraNo = ak.SiraNo,
                        AktifMi = ak.AktifMi
                    })
                .ToList();
        }

        public async Task<IReadOnlyList<AnasayfaKategoriDto>> TumunuGetirAsync()
        {
            var anasayfaKategorileri =
                await _anasayfaKategoriDeposu.TumunuGetirAsync();

            var kategoriler =
                await _kategoriDeposu.TumunuGetirAsync();

            return anasayfaKategorileri
                .OrderBy(x => x.SiraNo)
                .Join(
                    kategoriler,
                    ak => ak.KategoriId,
                    k => k.Id,
                    (ak, k) => new AnasayfaKategoriDto
                    {
                        Id = ak.Id,
                        KategoriId = k.Id,
                        KategoriAdi = k.Ad,
                        SeoUrl = k.SeoUrl,
                        GorselUrl = ak.GorselUrl ?? k.GorselUrl,
                        VitrinGorselUrl = ak.GorselUrl,
                        Aciklama = k.Aciklama,
                        SiraNo = ak.SiraNo,
                        AktifMi = ak.AktifMi
                    })
                .ToList();
        }

        public async Task<AnasayfaKategoriDto?> IdIleGetirAsync(Guid id)
        {
            var anasayfaKategori =
                await _anasayfaKategoriDeposu.IdIleGetirAsync(id);

            if (anasayfaKategori == null)
                return null;

            var kategori =
                await _kategoriDeposu.IdIleGetirAsync(
                    anasayfaKategori.KategoriId);

            if (kategori == null)
                return null;

            return new AnasayfaKategoriDto
            {
                Id = anasayfaKategori.Id,
                KategoriId = kategori.Id,
                KategoriAdi = kategori.Ad,
                SeoUrl = kategori.SeoUrl,
                GorselUrl = anasayfaKategori.GorselUrl ?? kategori.GorselUrl,
                VitrinGorselUrl = anasayfaKategori.GorselUrl,
                Aciklama = kategori.Aciklama,
                SiraNo = anasayfaKategori.SiraNo,
                AktifMi = anasayfaKategori.AktifMi
            };
        }

        public async Task<Guid> EkleAsync(AnasayfaKategoriEkleDto dto)
        {
            var kategori =
                await _kategoriDeposu.IdIleGetirAsync(dto.KategoriId);

            if (kategori == null)
                throw new InvalidOperationException(
                    "Seçilen kategori bulunamadı.");

            var mevcutKategoriler =
                await _anasayfaKategoriDeposu.TumunuGetirAsync();

            var zatenEkli =
                mevcutKategoriler.Any(x => x.KategoriId == dto.KategoriId);

            if (zatenEkli)
                throw new InvalidOperationException(
                    "Bu kategori zaten anasayfa kategorilerine eklenmiş.");

            var entity = new AnasayfaKategori
            {
                Id = Guid.NewGuid(),
                KategoriId = dto.KategoriId,
                GorselUrl = dto.GorselUrl,
                SiraNo = dto.SiraNo,
                AktifMi = dto.AktifMi
            };

            await _anasayfaKategoriDeposu.EkleAsync(entity);
            await _anasayfaKategoriDeposu.DegisiklikleriKaydetAsync();

            return entity.Id;
        }

        public async Task GuncelleAsync(
            AnasayfaKategoriGuncelleDto dto)
        {
            var entity =
                await _anasayfaKategoriDeposu.IdIleGetirAsync(dto.Id);

            if (entity == null)
                return;

            var kategori =
                await _kategoriDeposu.IdIleGetirAsync(dto.KategoriId);

            if (kategori == null)
                throw new InvalidOperationException(
                    "Seçilen kategori bulunamadı.");

            var mevcutKategoriler =
                await _anasayfaKategoriDeposu.TumunuGetirAsync();

            var zatenEkli = mevcutKategoriler.Any(x =>
                x.Id != dto.Id &&
                x.KategoriId == dto.KategoriId);

            if (zatenEkli)
                throw new InvalidOperationException(
                    "Bu kategori zaten anasayfa kategorilerine eklenmiş.");

            entity.KategoriId = dto.KategoriId;
            entity.GorselUrl = dto.GorselUrl;
            entity.SiraNo = dto.SiraNo;
            entity.AktifMi = dto.AktifMi;

            _anasayfaKategoriDeposu.Guncelle(entity);

            await _anasayfaKategoriDeposu.DegisiklikleriKaydetAsync();
        }

        public async Task SilAsync(Guid id)
        {
            var entity =
                await _anasayfaKategoriDeposu.IdIleGetirAsync(id);

            if (entity == null)
                return;

            _anasayfaKategoriDeposu.Sil(entity);

            await _anasayfaKategoriDeposu.DegisiklikleriKaydetAsync();
        }

        public async Task SiralariGuncelleAsync(List<Guid> idler)
        {
            var tumu =
                await _anasayfaKategoriDeposu.TumunuGetirAsync();

            for (int i = 0; i < idler.Count; i++)
            {
                var entity =
                    tumu.FirstOrDefault(x => x.Id == idler[i]);

                if (entity != null)
                {
                    entity.SiraNo = i + 1;
                    _anasayfaKategoriDeposu.Guncelle(entity);
                }
            }

            await _anasayfaKategoriDeposu.DegisiklikleriKaydetAsync();
        }
    }
}
