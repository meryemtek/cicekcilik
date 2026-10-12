using ECommerce.Application.DTOs.Kampanya;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces
{
    public interface IKampanyaServisi
    {
        Task<List<KampanyaUrunDto>> GetKampanyaUrunlerAsync();
        Task<List<KampanyaUrunDto>> GetAktifKampanyaUrunlerAsync();
        Task AddUrunToKampanyaAsync(KampanyaUrunEkleDto dto);
        Task UpdateKampanyaUrunAsync(KampanyaUrunGuncelleDto dto);
        Task RemoveUrunFromKampanyaAsync(Guid id);
    }
}