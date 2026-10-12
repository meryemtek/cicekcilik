namespace ECommerce.Application.Models
{
    public class SepetKalemi
    {
        public Guid UrunId { get; set; }
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UrunAdi { get; set; }
        public Guid? VaryantId { get; set; }
        public string VaryantAdi { get; set; }
        public decimal BirimFiyat { get; set; }
        public int Miktar { get; set; }
        public string GorselUrl { get; set; }
        public string Il { get; set; }
        public string Ilce { get; set; }
        public string Mahalle { get; set; }
        public string TeslimatGunu { get; set; }
        public string TeslimatSaati { get; set; }
        public bool VipTeslimatAktifMi { get; set; }
        public decimal SecilenEkUcretlerToplamI { get; set; }

        // Sipariş özetinde ayrı ayrı gösterebilmek için eklenen alanlar:
        public decimal TeslimatUcreti { get; set; }
        public decimal VipUcreti { get; set; }

        // Kalemin genel toplam fiyatı (Ürün + Miktar + Teslimat + VIP)
        public decimal ToplamFiyat => (BirimFiyat * Miktar) + TeslimatUcreti + VipUcreti;
    }
}