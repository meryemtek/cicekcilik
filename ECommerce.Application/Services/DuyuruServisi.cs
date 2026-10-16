using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.Application.DTOs.Duyuru;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;

namespace ECommerce.Infrastructure.Services
{
    public class DuyuruServisi : IDuyuruServisi
    {
        private readonly IDepo<Duyuru> _duyuruDeposu;

        public DuyuruServisi(IDepo<Duyuru> duyuruDeposu)
        {
            _duyuruDeposu = duyuruDeposu;
        }

        public async Task<IReadOnlyList<DuyuruDto>> AktifDuyurulariGetirAsync()
        {
            var liste = await _duyuruDeposu.GetirAsync(x => x.AktifMi);
            return liste.OrderBy(x => x.SiraNo).Select(x => new DuyuruDto
            {
                Id = x.Id,
                Metin = x.Metin,
                LinkUrl = x.LinkUrl,
                SiraNo = x.SiraNo,
                AktifMi = x.AktifMi
            }).ToList();
        }

        public async Task<IReadOnlyList<DuyuruDto>> TumDuyurulariGetirAsync()
        {
            var liste = await _duyuruDeposu.TumunuGetirAsync();
            return liste.OrderBy(x => x.SiraNo).Select(x => new DuyuruDto
            {
                Id = x.Id,
                Metin = x.Metin,
                LinkUrl = x.LinkUrl,
                SiraNo = x.SiraNo,
                AktifMi = x.AktifMi
            }).ToList();
        }

        public async Task DuyuruSilAsync(Guid id)
        {
            var entity = await _duyuruDeposu.IdIleGetirAsync(id);
            if (entity != null)
            {
                _duyuruDeposu.Sil(entity);
                await _duyuruDeposu.DegisiklikleriKaydetAsync();
            }
        }

        public async Task<DuyuruGuncelleDto?> IdIleGetirAsync(Guid id)
        {
            var entity = await _duyuruDeposu.IdIleGetirAsync(id);
            if (entity == null) return null;

            return new DuyuruGuncelleDto
            {
                Id = entity.Id,
                Metin = entity.Metin,
                LinkUrl = entity.LinkUrl,
                SiraNo = entity.SiraNo,
                AktifMi = entity.AktifMi
            };
        }

        public async Task<Guid> DuyuruEkleAsync(DuyuruEkleDto dto)
        {
            var entity = new Duyuru
            {
                Id = Guid.NewGuid(),
                Metin = dto.Metin,
                LinkUrl = dto.LinkUrl ?? string.Empty,
                SiraNo = dto.SiraNo,
                AktifMi = dto.AktifMi
            };
            await _duyuruDeposu.EkleAsync(entity);
            await _duyuruDeposu.DegisiklikleriKaydetAsync();
            return entity.Id;
        }

        public async Task DuyuruGuncelleAsync(DuyuruGuncelleDto dto)
        {
            var entity = await _duyuruDeposu.IdIleGetirAsync(dto.Id);
            if (entity != null)
            {
                entity.Metin = dto.Metin;
                if (dto.LinkUrl != null)
                    entity.LinkUrl = dto.LinkUrl;
                entity.SiraNo = dto.SiraNo;
                entity.AktifMi = dto.AktifMi;

                _duyuruDeposu.Guncelle(entity);
                await _duyuruDeposu.DegisiklikleriKaydetAsync();
            }
        }

    }
}
