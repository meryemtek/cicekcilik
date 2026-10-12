using ECommerce.Application.DTOs.IletisimMesaji;

namespace ECommerce.Application.Interfaces;

public interface IIletisimMesajiServisi
{
    Task<IReadOnlyList<IletisimMesajiListeDto>> TumMesajlariGetirAsync();
    Task<IletisimMesajiDetayDto?> MesajOkuAsync(Guid id);
    Task<Guid> EkleAsync(IletisimMesajiEkleDto dto);
    Task SilAsync(Guid id);
    Task<int> OkunmamisMesajSayisiGetirAsync();
}