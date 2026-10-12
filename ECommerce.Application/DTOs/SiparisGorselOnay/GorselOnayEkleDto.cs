using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.SiparisGorselOnay
{
    public class GorselOnayEkleDto
    {
        public Guid SiparisId { get; set; }
        public string GorselUrl { get; set; } = string.Empty;
    }
}
