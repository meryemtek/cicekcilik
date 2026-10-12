using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.OdemeBildirimi
{
    public class OdemeBildirimiListeDto
    {
        public Guid Id { get; set; }
        public string SiparisNo { get; set; } = string.Empty;
        public string AdSoyad { get; set; } = string.Empty;
        public string BankaAdi { get; set; } = string.Empty;
        public decimal Tutar { get; set; }
        public DateTime OdemeTarihi { get; set; }
        public DateTime OlusturulmaTarihi { get; set; }
        public bool OkunduMu { get; set; }
    }
}
