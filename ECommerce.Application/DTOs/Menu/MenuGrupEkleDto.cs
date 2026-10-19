using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.Menu
{
    public class MenuGrupEkleDto
    {
        public string Baslik { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public string Konum { get; set; } = "Header";
        public bool AktifMi { get; set; } = true;
    }
}
