using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.KartNotuSablonu
{
    public class KartNotuSablonuEkleDto
    {
        public string Kategori { get; set; } = string.Empty;
        public string MesajIcerigi { get; set; } = string.Empty;
        public int SiraNo { get; set; }
        public bool AktifMi { get; set; } = true;
    }
}
