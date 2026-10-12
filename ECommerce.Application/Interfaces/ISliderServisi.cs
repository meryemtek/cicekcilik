using ECommerce.Application.DTOs.Slider;

namespace ECommerce.Application.Interfaces;

public interface ISliderServisi
{
    Task<IReadOnlyList<SliderListeDto>> AktifSliderlariGetirAsync();
    Task<IReadOnlyList<SliderListeDto>> TumSliderlariGetirAsync();
    Task<SliderGuncelleDto?> IdIleGetirAsync(Guid id);
    Task<Guid> SliderEkleAsync(SliderEkleDto dto);
    Task SliderGuncelleAsync(SliderGuncelleDto dto);
    Task SliderSilAsync(Guid id);
}