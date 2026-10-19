using System.Threading.Tasks;
using ECommerce.Application.DTOs.Odeme;

namespace ECommerce.Application.Interfaces;

public interface IOdemeServisi
{
    Task<OdemeYanitDto> OdemeBaslatAsync(OdemeIstekDto istek);
    Task<bool> PaytrBildirimHashGecerliMiAsync(string siparisNumarasi, string durum, string toplamTutar, string hash);
}
