using Microsoft.AspNetCore.Http;

namespace ECommerce.Application.DTOs.OdemeBildirimi
{
    public class OdemeBildirimiEkleDto
    {
        public string SiparisNo { get; set; } = string.Empty;
        public string AdSoyad { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string BankaAdi { get; set; } = string.Empty;
        public decimal Tutar { get; set; }
        public string OdemeTarihi { get; set; } = string.Empty; // string olarak güncellendi
        public IFormFile? Dekont { get; set; }
        public string Aciklama { get; set; } = string.Empty;
    }
}