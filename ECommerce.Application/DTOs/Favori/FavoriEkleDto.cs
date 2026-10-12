using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.Favori
{
    public class FavoriEkleDto
    {
        public string KullaniciId { get; set; } = string.Empty;
        public Guid UrunId { get; set; }
    }
}
