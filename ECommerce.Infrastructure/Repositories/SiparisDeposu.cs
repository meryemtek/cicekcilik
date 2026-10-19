using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class SiparisDeposu : Depo<Siparis>, ISiparisDeposu
{
    private readonly UygulamaDbContext _uygulamaDbContext;

    public SiparisDeposu(UygulamaDbContext uygulamaDbContext) : base(uygulamaDbContext)
    {
        _uygulamaDbContext = uygulamaDbContext;
    }

    public async Task<Siparis?> SiparisDetayiGetirAsync(Guid id)
    {
        return await _uygulamaDbContext.Siparisler
            .Include(s => s.SiparisKalemleri)
                .ThenInclude(k => k.Urun)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Siparis?> SiparisNumarasiylaDetayGetirAsync(string siparisNumarasi)
    {
        return await _uygulamaDbContext.Siparisler
            .Include(s => s.SiparisKalemleri)
                .ThenInclude(k => k.Urun)
            .FirstOrDefaultAsync(s => s.SiparisNumarasi == siparisNumarasi);
    }
}
