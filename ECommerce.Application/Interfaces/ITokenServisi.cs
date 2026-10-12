using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces;

public interface ITokenServisi
{
    Task<string> TokenUretAsync(UygulamaKullanicisi kullanici);
}