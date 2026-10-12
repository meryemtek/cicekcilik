using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.Menu
{
    public class MenuGrupListeDto
    {
        public Guid Id { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string Konum { get; set; } = string.Empty;
        public bool AktifMi { get; set; }
        public bool SistemMi { get; set; } // Arayüzde sil butonunu gizlemek için
        public int ElemanSayisi { get; set; }
    }
}
