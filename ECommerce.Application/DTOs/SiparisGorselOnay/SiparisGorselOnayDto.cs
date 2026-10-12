using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.SiparisGorselOnay
{
    public class SiparisGorselOnayDto
    {
        public Guid Id { get; set; }
        public Guid SiparisId { get; set; }
        public string GorselUrl { get; set; } = string.Empty;
        public DateTime YuklemeTarihi { get; set; }
        public bool MusteriOnayladiMi { get; set; }
    }
}
