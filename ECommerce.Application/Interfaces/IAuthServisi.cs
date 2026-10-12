using ECommerce.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces
{
    public interface IAuthServisi
    {
        Task<TokenDto> KayitOlAsync(KayitDto dto);
        Task<TokenDto> GirisYapAsync(GirisDto dto);
    }
}
