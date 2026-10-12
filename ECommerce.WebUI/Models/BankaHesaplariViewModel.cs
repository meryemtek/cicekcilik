using ECommerce.Application.DTOs.BankaHesabi;
using ECommerce.Application.DTOs.OdemeBildirimi;

namespace ECommerce.WebUI.Models
{
    public class BankaHesaplariViewModel
    {
        public List<BankaHesabiPublicDto> Bankalar { get; set; } = new();
        public OdemeBildirimiEkleDto BildirimDto { get; set; } = new();
    }
}