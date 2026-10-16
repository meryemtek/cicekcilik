using ECommerce.Application.DTOs.Menu;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services
{
    public class MenuServisi : IMenuServisi
    {
        private readonly IDepo<MenuGrup> _grupDeposu;
        private readonly IDepo<MenuItem> _itemDeposu;

        public MenuServisi(IDepo<MenuGrup> grupDeposu, IDepo<MenuItem> itemDeposu)
        {
            _grupDeposu = grupDeposu;
            _itemDeposu = itemDeposu;
        }

        public async Task<IReadOnlyList<MenuGrupListeDto>> TumGruplariGetirAsync()
        {
            var gruplar = await _grupDeposu.TumunuGetirAsync();
            var sonuc = new List<MenuGrupListeDto>();

            foreach (var g in gruplar)
            {
                var itemler = await _itemDeposu.GetirAsync(i => i.MenuGrupId == g.Id);
                sonuc.Add(new MenuGrupListeDto
                {
                    Id = g.Id,
                    Baslik = g.Baslik,
                    Konum = g.Konum,
                    AktifMi = g.AktifMi,
                    SistemMi = g.SistemMi, // DTO - Entity mapping eklendi
                    ElemanSayisi = itemler.Count
                });
            }
            return sonuc;
        }

        public async Task<MenuGrupGuncelleDto?> GrupIdIleGetirAsync(Guid id)
        {
            var grup = await _grupDeposu.IdIleGetirAsync(id);
            if (grup == null) return null;

            return new MenuGrupGuncelleDto
            {
                Id = grup.Id,
                Baslik = grup.Baslik,
                Konum = grup.Konum,
                AktifMi = grup.AktifMi,
                SistemMi = grup.SistemMi
            };
        }

        public async Task<Guid> GrupEkleAsync(MenuGrupEkleDto dto)
        {
            var grup = new MenuGrup
            {
                Baslik = dto.Baslik,
                Konum = dto.Konum,
                AktifMi = dto.AktifMi,
                SistemMi = false // Manuel eklenenler sistem grubu olamaz
            };
            await _grupDeposu.EkleAsync(grup);
            await _grupDeposu.DegisiklikleriKaydetAsync();
            return grup.Id;
        }

        public async Task GrupGuncelleAsync(MenuGrupGuncelleDto dto)
        {
            var grup = await _grupDeposu.IdIleGetirAsync(dto.Id);
            if (grup != null)
            {
                grup.Baslik = dto.Baslik;
                grup.Konum = dto.Konum;
                grup.AktifMi = dto.AktifMi;
                grup.GuncellenmeTarihi = DateTime.UtcNow;

                _grupDeposu.Guncelle(grup);
                await _grupDeposu.DegisiklikleriKaydetAsync();
            }
        }

        public async Task GrupSilAsync(Guid id)
        {
            var grup = await _grupDeposu.IdIleGetirAsync(id);
            if (grup != null)
            {
                if (grup.SistemMi)
                {
                    throw new InvalidOperationException("Sistem menü grupları silinemez.");
                }

                _grupDeposu.Sil(grup);
                await _grupDeposu.DegisiklikleriKaydetAsync();
            }
        }

        public async Task GrupDurumDegistirAsync(Guid id, bool aktifMi)
        {
            var grup = await _grupDeposu.IdIleGetirAsync(id);
            if (grup != null)
            {
                grup.AktifMi = aktifMi;
                grup.GuncellenmeTarihi = DateTime.UtcNow;
                _grupDeposu.Guncelle(grup);
                await _grupDeposu.DegisiklikleriKaydetAsync();
            }
        }

        public async Task<IReadOnlyList<MenuItemListeDto>> GrubaAitMenuleriGetirAsync(Guid menuGrupId)
        {
            var itemler = await _itemDeposu.GetirAsync(i => i.MenuGrupId == menuGrupId);

            return itemler
                .Where(i => i.ParentId == null)
                .OrderBy(i => i.SiraNo)
                .Select(i => MapToHiyerarsikDto(i, itemler.ToList()))
                .ToList();
        }

        public async Task<MenuItemGuncelleDto?> MenuItemIdIleGetirAsync(Guid id)
        {
            var item = await _itemDeposu.IdIleGetirAsync(id);
            if (item == null) return null;

            return new MenuItemGuncelleDto
            {
                Id = item.Id,
                MenuGrupId = item.MenuGrupId,
                Baslik = item.Baslik,
                Url = item.Url,
                IconClass = item.IconClass,
                Target = item.Target,
                SiraNo = item.SiraNo,
                AktifMi = item.AktifMi,
                ParentId = item.ParentId
            };
        }

        private MenuItemListeDto MapToHiyerarsikDto(MenuItem item, List<MenuItem> tumItemler)
        {
            return new MenuItemListeDto
            {
                Id = item.Id,
                MenuGrupId = item.MenuGrupId,
                Baslik = item.Baslik,
                Url = item.Url,
                IconClass = item.IconClass,
                Target = item.Target,
                SiraNo = item.SiraNo,
                AktifMi = item.AktifMi,
                ParentId = item.ParentId,
                AltMenuler = tumItemler
                    .Where(alt => alt.ParentId == item.Id)
                    .OrderBy(alt => alt.SiraNo)
                    .Select(alt => MapToHiyerarsikDto(alt, tumItemler))
                    .ToList()
            };
        }

        public async Task<Guid> MenuItemEkleAsync(MenuItemEkleDto dto)
        {
            var item = new MenuItem
            {
                MenuGrupId = dto.MenuGrupId,
                Baslik = dto.Baslik,
                Url = dto.Url,
                IconClass = dto.IconClass,
                Target = dto.Target,
                SiraNo = dto.SiraNo,
                AktifMi = dto.AktifMi,
                ParentId = dto.ParentId
            };
            await _itemDeposu.EkleAsync(item);
            await _itemDeposu.DegisiklikleriKaydetAsync();
            return item.Id;
        }

        public async Task MenuItemGuncelleAsync(MenuItemGuncelleDto dto)
        {
            var item = await _itemDeposu.IdIleGetirAsync(dto.Id);
            if (item != null)
            {
                item.MenuGrupId = dto.MenuGrupId;
                item.Baslik = dto.Baslik;
                item.Url = dto.Url;
                item.IconClass = dto.IconClass;
                item.Target = dto.Target;
                item.SiraNo = dto.SiraNo;
                item.AktifMi = dto.AktifMi;
                item.ParentId = dto.ParentId;
                item.GuncellenmeTarihi = DateTime.UtcNow;

                _itemDeposu.Guncelle(item);
                await _itemDeposu.DegisiklikleriKaydetAsync();
            }
        }

        public async Task MenuItemSilAsync(Guid id)
        {
            var item = await _itemDeposu.IdIleGetirAsync(id);
            if (item != null)
            {
                var altItemler = await _itemDeposu.GetirAsync(i => i.ParentId == id);
                foreach (var alt in altItemler)
                {
                    alt.ParentId = null;
                    _itemDeposu.Guncelle(alt);
                }

                _itemDeposu.Sil(item);
                await _itemDeposu.DegisiklikleriKaydetAsync();
            }
        }

        public async Task<IReadOnlyList<MenuItemListeDto>> KonumaGoreMenuleriGetirAsync(string konum = "Header")
        {
            var gruplar = await _grupDeposu.GetirAsync(g => g.Konum == konum && g.AktifMi);
            var aktifGrup = gruplar.FirstOrDefault();
            if (aktifGrup == null) return new List<MenuItemListeDto>();

            var itemler = await _itemDeposu.GetirAsync(i => i.MenuGrupId == aktifGrup.Id && i.AktifMi);

            return itemler
                .Where(i => i.ParentId == null)
                .OrderBy(i => i.SiraNo)
                .Select(i => MapToHiyerarsikDto(i, itemler.ToList()))
                .ToList();
        }
    }
}