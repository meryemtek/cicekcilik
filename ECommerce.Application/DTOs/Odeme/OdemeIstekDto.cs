using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.Odeme
{
    public class OdemeIstekDto
    {
        public string SiparisNumarasi { get; set; } = string.Empty;
        public decimal ToplamTutar { get; set; }
        public string MusteriEmail { get; set; } = string.Empty;
        public string MusteriAdSoyad { get; set; } = string.Empty;
        public string MusteriTelefon { get; set; } = string.Empty;
        public string IPAdresi { get; set; } = string.Empty;
        public List<OdemeUrunDto> Urunler { get; set; } = new List<OdemeUrunDto>();
    }
}
