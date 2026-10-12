using System;
using System.Collections.Generic;

namespace ECommerce.Application.DTOs.Menu
{
    public class MenuItemListeDto
    {
        public Guid Id { get; set; }
        public Guid MenuGrupId { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? IconClass { get; set; }
        public string Target { get; set; } = "_self";
        public int SiraNo { get; set; }
        public bool AktifMi { get; set; }
        public Guid? ParentId { get; set; }
        public List<MenuItemListeDto> AltMenuler { get; set; } = new List<MenuItemListeDto>();
    }
}