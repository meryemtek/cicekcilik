using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.SemtTeslimatBolgesi
{
    public class SemtBolgeGuncelleDto
    {
        public Guid Id { get; set; }
        public string IlAdi { get; set; } = string.Empty;
        public string IlceAdi { get; set; } = string.Empty;
        public string MahalleAdi { get; set; } = string.Empty;
        public decimal EkTeslimatUcreti { get; set; }
        public bool AktifMi { get; set; }
    }
}
