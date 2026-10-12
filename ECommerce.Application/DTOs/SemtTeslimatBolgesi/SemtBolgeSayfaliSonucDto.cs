using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.SemtTeslimatBolgesi
{
    public class SemtBolgeSayfaliSonucDto
    {
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public List<SemtBolgeListeDto> Data { get; set; } = new List<SemtBolgeListeDto>();
    }
}
