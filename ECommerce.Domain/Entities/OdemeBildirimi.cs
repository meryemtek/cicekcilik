namespace ECommerce.Domain.Entities
{
    public class OdemeBildirimi : TemelVarlik
    {
        public string SiparisNo { get; set; } = string.Empty;
        public string AdSoyad { get; set; } = string.Empty; // Gönderen Ad Soyad
        public string Telefon { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string BankaAdi { get; set; } = string.Empty;
        public decimal Tutar { get; set; }
        public DateTime OdemeTarihi { get; set; }
        public string? DekontYolu { get; set; } // Yüklenen dekontun sunucu yolu
        public string Aciklama { get; set; } = string.Empty;
        public bool OkunduMu { get; set; } = false;
    }
}