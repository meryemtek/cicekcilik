
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities
{
    public class TeslimatSaati : TemelVarlik
    {
        public Guid Id { get; set; }
        public string BaslangicSaati { get; set; } = string.Empty; // Örn: "09:00"
        public string BitisSaati { get; set; } = string.Empty;     // Örn: "12:00"
        public string Aciklama { get; set; } = string.Empty;       // Örn: "Sabah Grubu (09:00 - 12:00)"
        public int GunlukKota { get; set; } = 50;                  // O saat diliminde alınabilecek maksimum sipariş
        public bool AktifMi { get; set; } = true;                  // Şu an kullanımda mı?
        public int Sira { get; set; } = 0;                         // Listeleme sırası
        public TeslimatGrubu TeslimatGrubu { get; set; }           // YENİ EKLENEN PROPERTY (Normal veya VIP olduğunu belirtir)
        public DateTime? GuncellenmeTarihi { get; set; }
    }
}