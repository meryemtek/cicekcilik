using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.Favori
{
    public class FavoriListeDto
    {
        public Guid Id { get; set; }
        public Guid UrunId { get; set; }
        public string UrunAdi { get; set; } = string.Empty;
        public string SeoUrl { get; set; } = string.Empty;
        public string GorselUrl { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public string? Etiket { get; set; }
    }
}
