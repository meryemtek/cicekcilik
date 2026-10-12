using ECommerce.Application.DTOs.AnasayfaKategori;

namespace ECommerce.Application.Interfaces
{
    public interface IAnasayfaKategoriServisi
    {
        Task<IReadOnlyList<AnasayfaKategoriDto>> AktifleriGetirAsync();
        Task<IReadOnlyList<AnasayfaKategoriDto>> TumunuGetirAsync();
        Task<AnasayfaKategoriDto?> IdIleGetirAsync(Guid id);
        Task<Guid> EkleAsync(AnasayfaKategoriEkleDto dto);
        Task GuncelleAsync(AnasayfaKategoriGuncelleDto dto);
        Task SilAsync(Guid id);
        Task SiralariGuncelleAsync(List<Guid> idler);
    }
}