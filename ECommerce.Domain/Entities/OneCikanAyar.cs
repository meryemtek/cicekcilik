using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class OneCikanAyar : TemelVarlik
    {
        public bool TumUrunleriGoster { get; set; } = true;
        public string? SecilenKategoriIdleri { get; set; } = string.Empty;
        public string? SecilenUrunIdleri { get; set; } = string.Empty;
    }
}
