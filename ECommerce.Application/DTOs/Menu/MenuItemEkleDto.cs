using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.DTOs.Menu
{
    public class MenuItemEkleDto
    {
        public Guid MenuGrupId { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? IconClass { get; set; }
        public string Target { get; set; } = "_self";
        public int SiraNo { get; set; }
        public bool AktifMi { get; set; } = true;
        public Guid? ParentId { get; set; }
    }
}
