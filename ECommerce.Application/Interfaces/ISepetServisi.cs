using ECommerce.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces;

public interface ISepetServisi
{
    Task<Sepet> SepetGetirAsync(string sepetId);
    Task<Sepet> SepeteUrunEkleAsync(string sepetId, SepetKalemi kalem);
    Task<Sepet> SepetFiyatlariniGuncelleAsync(string sepetId);
    Task<Sepet> SepettenUrunCikarAsync(string sepetId, Guid urunId);
    Task<Sepet> UrunMiktariGuncelleAsync(string sepetId, Guid urunId, int miktar);
    Task<Sepet> SepetUrunleriniBirlestirAsync(string sepetId);
    Task<Sepet> UrunuAyirAsync(string sepetId, Guid kalemId);
    Task SepetiTemizleAsync(string sepetId);
    Task<Sepet> KuponUygulaAsync(string sepetId, string kuponKodu);
    Task<Sepet> KuponKaldirAsync(string sepetId);
}
