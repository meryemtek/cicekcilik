using System;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces;

public interface IHiCariEntegrasyonServisi
{
    Task<bool> CariVeStokIsleAsync(Guid siparisId);
}