using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class KategoriBanner : TemelVarlik
    {
        public string Baslik { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public string GorselUrl { get; set; } = string.Empty;
        public Guid KategoriId { get; set; } // Hangi kategoriye gideceği
        public int Sira { get; set; } = 0;
        public bool AktifMi { get; set; } = true;
    }
}
