using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.BankaHesabi
{
    public class BankaHesabiPublicDto
    {
        public Guid Id { get; set; }
        public string BankaAdi { get; set; } = string.Empty;
        public string SubeKodu { get; set; } = string.Empty;
        public string HesapNo { get; set; } = string.Empty;
        public string Iban { get; set; } = string.Empty;
        public string AliciAdSoyad { get; set; } = string.Empty;
    }
}
