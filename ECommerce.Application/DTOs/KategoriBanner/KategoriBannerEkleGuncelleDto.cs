using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.KategoriBanner
{
    public class KategoriBannerEkleGuncelleDto
    {
        public Guid Id { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public string GorselUrl { get; set; } = string.Empty;
        public Guid KategoriId { get; set; }
        public int Sira { get; set; }
        public bool AktifMi { get; set; } = true;
    }
}
