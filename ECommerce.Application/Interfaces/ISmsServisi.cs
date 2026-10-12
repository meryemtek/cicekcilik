using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces;

public interface ISmsServisi
{
    Task<bool> SmsGonderAsync(string telefonNumarasi, string mesaj);
}