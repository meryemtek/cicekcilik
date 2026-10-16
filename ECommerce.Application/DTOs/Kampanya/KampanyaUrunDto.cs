using System;

namespace ECommerce.Application.DTOs.Kampanya
{
    public class KampanyaUrunDto
    {
        public Guid Id { get; set; }
        public string KampanyaAdi { get; set; } = null!;
        public string? KampanyaAciklamasi { get; set; }
        public string KapsamTuru { get; set; } = null!;

        public Guid? UrunId { get; set; }
        public string UrunAdi { get; set; } = null!;
        public string UrunKodu { get; set; } = null!;
        public decimal Fiyat { get; set; }
        public string? GorselUrl { get; set; }
        public string SeoUrl { get; set; } = null!;

        // Kategori ve Çapraz Kampanya Destek Alanları
        public Guid? TetikleyiciKategoriId { get; set; }
        public string? KategoriAdi { get; set; }
        public string? KategoriSeoUrl { get; set; }
        public Guid? HedefKategoriId { get; set; }

        public string IndirimTipi { get; set; } = null!;
        public decimal IndirimDegeri { get; set; }
        public decimal IndirimliFiyat => IndirimTipi == "Yuzde"
            ? Math.Max(0, Math.Round(Fiyat - (Fiyat * IndirimDegeri / 100m), 2))
            : Math.Max(0, Math.Round(Fiyat - IndirimDegeri, 2));
        public string KampanyaVaryasyonu { get; set; } = null!;
        public bool KuponlarlaBirlesebilirMi { get; set; }
        public int Sira { get; set; }
        public bool AktifMi { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
    }
}