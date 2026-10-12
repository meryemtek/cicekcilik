using System;
using System.Collections.Generic;

namespace ECommerce.Application.DTOs.Kampanya
{
    public class KampanyaUrunEkleDto
    {
        public string KampanyaAdi { get; set; } = null!;
        public string? KampanyaAciklamasi { get; set; }

        public string KapsamTuru { get; set; } = "Urun";
        public List<Guid> UrunIds { get; set; } = new List<Guid>();

        // Kategori ve Çapraz Kampanya Destek Alanları
        public Guid? KategoriId { get; set; }
        public Guid? TetikleyiciKategoriId { get; set; }
        public Guid? HedefKategoriId { get; set; }
        public List<Guid> HedefUrunIds { get; set; } = new List<Guid>();
        public bool KuponlarlaBirlesebilirMi { get; set; }
        public string IndirimTipi { get; set; } = "Yuzde";
        public decimal IndirimDegeri { get; set; }

        public string KampanyaVaryasyonu { get; set; } = "DuzIndirim";
        public int Sira { get; set; } = 0;
        public bool AktifMi { get; set; } = true;
        public DateTime BaslangicTarihi { get; set; } = DateTime.Now;
        public DateTime? BitisTarihi { get; set; }
    }
}