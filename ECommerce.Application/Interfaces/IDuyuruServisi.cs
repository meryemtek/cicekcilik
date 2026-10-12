using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerce.Application.DTOs.Duyuru;

namespace ECommerce.Application.Interfaces
{
    public interface IDuyuruServisi
    {
        Task<IReadOnlyList<DuyuruDto>> AktifDuyurulariGetirAsync();
        Task<IReadOnlyList<DuyuruDto>> TumDuyurulariGetirAsync();
        Task<DuyuruGuncelleDto?> IdIleGetirAsync(Guid id); // Admin panelinde düzenleme ekranı için
        Task<Guid> DuyuruEkleAsync(DuyuruEkleDto dto);
        Task DuyuruGuncelleAsync(DuyuruGuncelleDto dto);
        Task DuyuruSilAsync(Guid id);
    }
}