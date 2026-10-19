using ECommerce.Application.DTOs.Menu;

namespace ECommerce.Application.Interfaces
{
    public interface IMenuServisi
    {
        // Grup İşlemleri
        Task<IReadOnlyList<MenuGrupListeDto>> TumGruplariGetirAsync();
        Task<IReadOnlyList<MenuGrupListeDto>> FooterBolumleriniGetirAsync();
        Task<MenuGrupGuncelleDto?> GrupIdIleGetirAsync(Guid id);
        Task<Guid> GrupEkleAsync(MenuGrupEkleDto dto);
        Task GrupGuncelleAsync(MenuGrupGuncelleDto dto);
        Task GrupSilAsync(Guid id);
        Task GrupDurumDegistirAsync(Guid id, bool aktifMi);

        // Menü Öğesi (Item) İşlemleri (Hiyerarşik Ağaç)
        Task<IReadOnlyList<MenuItemListeDto>> GrubaAitMenuleriGetirAsync(Guid menuGrupId);
        Task<MenuItemGuncelleDto?> MenuItemIdIleGetirAsync(Guid id);
        Task<Guid> MenuItemEkleAsync(MenuItemEkleDto dto);
        Task MenuItemGuncelleAsync(MenuItemGuncelleDto dto);
        Task MenuItemSilAsync(Guid id);
        Task<IReadOnlyList<MenuItemListeDto>> KonumaGoreMenuleriGetirAsync(string konum = "Header");
    }
}
