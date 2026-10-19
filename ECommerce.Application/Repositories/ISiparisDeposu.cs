using ECommerce.Domain.Entities;

namespace ECommerce.Application.Repositories;

public interface ISiparisDeposu : IDepo<Siparis>
{
    Task<Siparis?> SiparisDetayiGetirAsync(Guid id);
    Task<Siparis?> SiparisNumarasiylaDetayGetirAsync(string siparisNumarasi);
}
