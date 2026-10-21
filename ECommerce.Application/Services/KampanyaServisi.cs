using ECommerce.Application.DTOs.Kampanya;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECommerce.Application.Services
{
    public class KampanyaServisi : IKampanyaServisi
    {
        private readonly IDepo<KampanyaUrun> _kampanyaDepo;
        private readonly IDepo<Urun> _urunDepo;
        private readonly IDepo<Kategori> _kategoriDepo;

        public KampanyaServisi(IDepo<KampanyaUrun> kampanyaDepo, IDepo<Urun> urunDepo, IDepo<Kategori> kategoriDepo)
        {
            _kampanyaDepo = kampanyaDepo;
            _urunDepo = urunDepo;
            _kategoriDepo = kategoriDepo;
        }

        public async Task<List<KampanyaUrunDto>> GetKampanyaUrunlerAsync()
        {
            var kampanyalar = (await _kampanyaDepo.TumunuGetirAsync()).OrderBy(k => k.Sira).ToList();
            if (!kampanyalar.Any()) return new List<KampanyaUrunDto>();

            var kampanyaUrunIds = kampanyalar.Where(k => k.UrunId.HasValue).Select(k => k.UrunId.Value).Distinct().ToList();
            var tumKategoriler = (await _kategoriDepo.TumunuGetirAsync()).ToDictionary(x => x.Id, x => x);

            // ÃœrÃ¼nleri sÃ¶zlÃ¼k yapÄ±sÄ±na gÃ¼venli bir ÅŸekilde aktarÄ±yoruz
            var tumUrunler = new Dictionary<Guid, Urun>();
            if (kampanyaUrunIds.Any())
            {
                var urunListesi = await _urunDepo.GetirAsync(u => kampanyaUrunIds.Contains(u.Id));
                foreach (var u in urunListesi)
                {
                    if (!tumUrunler.ContainsKey(u.Id))
                    {
                        tumUrunler.Add(u.Id, u);
                    }
                }
            }

            var sonuc = new List<KampanyaUrunDto>();
            foreach (var k in kampanyalar)
            {
                Urun? urun = null;
                if (k.UrunId.HasValue)
                {
                    tumUrunler.TryGetValue(k.UrunId.Value, out urun);
                }

                Kategori? kategori = null;
                if (k.KategoriId.HasValue)
                {
                    tumKategoriler.TryGetValue(k.KategoriId.Value, out kategori);
                }
                else if (urun != null && tumKategoriler.TryGetValue(urun.KategoriId, out var urunKat))
                {
                    kategori = urunKat;
                }

                sonuc.Add(new KampanyaUrunDto
                {
                    Id = k.Id,
                    KampanyaAdi = k.KampanyaAdi,
                    KampanyaAciklamasi = k.KampanyaAciklamasi,
                    KapsamTuru = k.KapsamTuru,
                    UrunId = urun?.Id,
                    UrunAdi = urun?.Ad ?? (k.KapsamTuru == "Kategori" && kategori != null ? $"{kategori.Ad} Kategorisi FÄ±rsatÄ±" : "Genel Kampanya"),
                    UrunKodu = urun?.UrunKodu ?? "",
                    Fiyat = urun?.Fiyat ?? 0,
                    GorselUrl = urun?.GorselUrl ?? kategori?.GorselUrl,
                    SeoUrl = urun?.SeoUrl ?? "",
                    TetikleyiciKategoriId = k.KategoriId,
                    KategoriAdi = kategori?.Ad,
                    KategoriSeoUrl = kategori?.SeoUrl,
                    HedefKategoriId = null,
                    IndirimTipi = k.IndirimTipi,
                    IndirimDegeri = k.IndirimDegeri,
                    KampanyaVaryasyonu = k.KampanyaVaryasyonu,
                    KuponlarlaBirlesebilirMi = k.KuponlarlaBirlesebilirMi,
                    Sira = k.Sira,
                    AktifMi = k.AktifMi,
                    BaslangicTarihi = k.BaslangicTarihi,
                    BitisTarihi = k.BitisTarihi
                });
            }
            return sonuc;
        }

        public async Task<List<KampanyaUrunDto>> GetAktifKampanyaUrunlerAsync()
        {
            var simdi = DateTime.UtcNow;
            var kampanyalar = (await _kampanyaDepo.GetirAsync(k => k.AktifMi && k.BaslangicTarihi <= simdi && (k.BitisTarihi == null || k.BitisTarihi > simdi)))
                                                 .OrderBy(k => k.Sira).ToList();

            if (!kampanyalar.Any()) return new List<KampanyaUrunDto>();

            var kampanyaUrunIds = kampanyalar.Where(k => k.UrunId.HasValue).Select(k => k.UrunId.Value).Distinct().ToList();
            var tumKategoriler = (await _kategoriDepo.TumunuGetirAsync()).ToDictionary(x => x.Id, x => x);

            var tumUrunler = new Dictionary<Guid, Urun>();
            if (kampanyaUrunIds.Any())
            {
                var urunListesi = await _urunDepo.GetirAsync(u => kampanyaUrunIds.Contains(u.Id) && u.AktifMi);
                foreach (var u in urunListesi)
                {
                    if (!tumUrunler.ContainsKey(u.Id))
                    {
                        tumUrunler.Add(u.Id, u);
                    }
                }
            }

            var sonuc = new List<KampanyaUrunDto>();
            foreach (var k in kampanyalar)
            {
                Urun? urun = null;
                if (k.UrunId.HasValue)
                {
                    tumUrunler.TryGetValue(k.UrunId.Value, out urun);
                }

                if (k.UrunId.HasValue && urun == null) continue;

                Kategori? kategori = null;
                if (k.KategoriId.HasValue)
                {
                    tumKategoriler.TryGetValue(k.KategoriId.Value, out kategori);
                }
                else if (urun != null && tumKategoriler.TryGetValue(urun.KategoriId, out var urunKat))
                {
                    kategori = urunKat;
                }

                sonuc.Add(new KampanyaUrunDto
                {
                    Id = k.Id,
                    KampanyaAdi = k.KampanyaAdi,
                    KampanyaAciklamasi = k.KampanyaAciklamasi,
                    KapsamTuru = k.KapsamTuru,
                    UrunId = urun?.Id,
                    UrunAdi = urun?.Ad ?? (k.KapsamTuru == "Kategori" && kategori != null ? $"{kategori.Ad} Kategorisi FÄ±rsatÄ±" : "Genel Kampanya"),
                    UrunKodu = urun?.UrunKodu ?? "",
                    Fiyat = urun?.Fiyat ?? 0,
                    GorselUrl = urun?.GorselUrl ?? kategori?.GorselUrl,
                    SeoUrl = urun?.SeoUrl ?? "",
                    TetikleyiciKategoriId = k.KategoriId,
                    KategoriAdi = kategori?.Ad,
                    KategoriSeoUrl = kategori?.SeoUrl,
                    HedefKategoriId = null,
                    IndirimTipi = k.IndirimTipi,
                    IndirimDegeri = k.IndirimDegeri,
                    KampanyaVaryasyonu = k.KampanyaVaryasyonu,
                    KuponlarlaBirlesebilirMi = k.KuponlarlaBirlesebilirMi,
                    Sira = k.Sira,
                    AktifMi = k.AktifMi,
                    BaslangicTarihi = k.BaslangicTarihi,
                    BitisTarihi = k.BitisTarihi
                });
            }
            return sonuc;
        }

        public async Task AddUrunToKampanyaAsync(KampanyaUrunEkleDto dto)
        {
            string? hedefKurallarJson = dto.HedefUrunIds != null && dto.HedefUrunIds.Any()
                ? JsonSerializer.Serialize(dto.HedefUrunIds)
                : null;

            if (dto.KampanyaVaryasyonu == "KategoriCapraz" && dto.TetikleyiciKategoriId.HasValue)
            {
                var varlik = new KampanyaUrun
                {
                    KampanyaAdi = dto.KampanyaAdi,
                    KampanyaAciklamasi = dto.KampanyaAciklamasi,
                    KapsamTuru = "Kategori",
                    UrunId = null,
                    KategoriId = dto.TetikleyiciKategoriId,
                    IndirimTipi = dto.IndirimTipi,
                    IndirimDegeri = dto.IndirimDegeri,
                    KampanyaVaryasyonu = dto.KampanyaVaryasyonu,
                    DinamikKurallarJson = hedefKurallarJson,
                    KuponlarlaBirlesebilirMi = dto.KuponlarlaBirlesebilirMi,
                    Sira = dto.Sira,
                    AktifMi = dto.AktifMi,
                    BaslangicTarihi = dto.BaslangicTarihi,
                    BitisTarihi = dto.BitisTarihi
                };
                await _kampanyaDepo.EkleAsync(varlik);
            }
            else if (dto.UrunIds != null && dto.UrunIds.Any())
            {
                foreach (var urunId in dto.UrunIds)
                {
                    var varlik = new KampanyaUrun
                    {
                        KampanyaAdi = dto.KampanyaAdi,
                        KampanyaAciklamasi = dto.KampanyaAciklamasi,
                        KapsamTuru = "Urun",
                        UrunId = urunId,
                        KategoriId = dto.KategoriId,
                        IndirimTipi = dto.IndirimTipi,
                        IndirimDegeri = dto.IndirimDegeri,
                        KampanyaVaryasyonu = dto.KampanyaVaryasyonu,
                        DinamikKurallarJson = hedefKurallarJson,
                        KuponlarlaBirlesebilirMi = dto.KuponlarlaBirlesebilirMi,
                        Sira = dto.Sira,
                        AktifMi = dto.AktifMi,
                        BaslangicTarihi = dto.BaslangicTarihi,
                        BitisTarihi = dto.BitisTarihi
                    };
                    await _kampanyaDepo.EkleAsync(varlik);
                }
            }
            await _kampanyaDepo.DegisiklikleriKaydetAsync();
        }

        public async Task UpdateKampanyaUrunAsync(KampanyaUrunGuncelleDto dto)
        {
            var varlik = await _kampanyaDepo.IdIleGetirAsync(dto.Id);
            if (varlik != null)
            {
                varlik.KampanyaAdi = dto.KampanyaAdi;
                varlik.KampanyaAciklamasi = dto.KampanyaAciklamasi;
                varlik.IndirimTipi = dto.IndirimTipi;
                varlik.IndirimDegeri = dto.IndirimDegeri;
                varlik.KampanyaVaryasyonu = dto.KampanyaVaryasyonu;
                varlik.KategoriId = dto.TetikleyiciKategoriId ?? varlik.KategoriId;
                varlik.KuponlarlaBirlesebilirMi = dto.KuponlarlaBirlesebilirMi;

                if (dto.HedefUrunIds != null && dto.HedefUrunIds.Any())
                {
                    varlik.DinamikKurallarJson = JsonSerializer.Serialize(dto.HedefUrunIds);
                }

                varlik.Sira = dto.Sira;
                varlik.AktifMi = dto.AktifMi;
                varlik.BaslangicTarihi = dto.BaslangicTarihi;
                varlik.BitisTarihi = dto.BitisTarihi;

                _kampanyaDepo.Guncelle(varlik);
                await _kampanyaDepo.DegisiklikleriKaydetAsync();
            }
        }

        public async Task RemoveUrunFromKampanyaAsync(Guid id)
        {
            var varlik = await _kampanyaDepo.IdIleGetirAsync(id);
            if (varlik != null)
            {
                _kampanyaDepo.Sil(varlik);
                await _kampanyaDepo.DegisiklikleriKaydetAsync();
            }
        }

        public async Task DurumDegistirAsync(Guid id, bool aktifMi)
        {
            var varlik = await _kampanyaDepo.IdIleGetirAsync(id);
            if (varlik != null)
            {
                varlik.AktifMi = aktifMi;
                varlik.GuncellenmeTarihi = DateTime.UtcNow;
                _kampanyaDepo.Guncelle(varlik);
                await _kampanyaDepo.DegisiklikleriKaydetAsync();
            }
        }
    }
}
