using ECommerce.Application.DTOs.Urun;

namespace ECommerce.Application.Interfaces;

public interface ITopluUrunYonetimServisi
{
    Task TopluFiyatGuncelleAsync(TopluFiyatGuncelleDto dto);
    Task TopluDurumDegistirAsync(bool aktifMi, Guid? kategoriId = null);
}