using ECommerce.Application.DTOs.TeslimatSaati;
using ECommerce.Domain.Enums;

public interface ITeslimatSaatiServisi
{
    Task<List<TeslimatSaatiDto>> AktifSaatleriGetirAsync(TeslimatGrubu? grup = null);
    Task<List<TeslimatSaatiDto>> TumunuGetirAsync();
    Task<TeslimatSaatiDto?> GetirByIdAsync(Guid id);
    Task EkleAsync(TeslimatSaatiDto dto);
    Task GuncelleAsync(TeslimatSaatiDto dto);
    Task SilAsync(Guid id);
}