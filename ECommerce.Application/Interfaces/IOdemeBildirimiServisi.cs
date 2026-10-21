using ECommerce.Application.DTOs.OdemeBildirimi;

namespace ECommerce.Application.Interfaces
{
    public interface IOdemeBildirimiServisi
    {
        Task<IReadOnlyList<OdemeBildirimiListeDto>> TumBildirimleriGetirAsync();
        Task<OdemeBildirimiDetayDto?> BildirimOkuAsync(Guid id);
        Task<Guid> EkleAsync(OdemeBildirimiEkleDto dto);
        Task SilAsync(Guid id);
        Task<int> OkunmamisBildirimSayisiGetirAsync();
    }
}
