using System;

namespace ECommerce.Domain.Entities
{
    public class KampanyaUrun : TemelVarlik
    {
        // Temel Bilgiler
        public string KampanyaAdi { get; set; } = null!;
        public string? KampanyaAciklamasi { get; set; }

        // Kapsam (Ürün Bazlı mı, Kategori Bazlı mı?)
        public string KapsamTuru { get; set; } = "Urun"; // "Urun" veya "Kategori"
        public Guid? UrunId { get; set; }
        public virtual Urun? Urun { get; set; }
        public Guid? KategoriId { get; set; }

        // İndirim Tipi ve Değeri
        public string IndirimTipi { get; set; } = "Yuzde"; // "Yuzde" veya "Tutar"
        public decimal IndirimDegeri { get; set; } // Örn: 20 (%20) veya 100 (100 TL)

        // Dinamik Senaryo / Varyasyon Türü
        public string KampanyaVaryasyonu { get; set; } = "DuzIndirim";

        // Dinamik Senaryo Parametreleri (JSON veya metin bazlı saklanabilir)
        public string? DinamikKurallarJson { get; set; }

        // Kupon Birleştirme Kuralı
        public bool KuponlarlaBirlesebilirMi { get; set; } = false; // Varsayılan olarak kuponla birleşmez

        public int Sira { get; set; } = 0;
        public bool AktifMi { get; set; } = true;
        public DateTime BaslangicTarihi { get; set; } = DateTime.UtcNow;
        public DateTime? BitisTarihi { get; set; }
    }
}
