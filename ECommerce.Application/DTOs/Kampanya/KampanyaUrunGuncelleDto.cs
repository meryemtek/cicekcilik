using System;
using System.Collections.Generic;

namespace ECommerce.Application.DTOs.Kampanya
{
    public class KampanyaUrunGuncelleDto
    {
        public Guid Id { get; set; }
        public string KampanyaAdi { get; set; } = null!;
        public string? KampanyaAciklamasi { get; set; }

        public string IndirimTipi { get; set; } = "Yuzde";
        public decimal IndirimDegeri { get; set; }
        public string KampanyaVaryasyonu { get; set; } = "DuzIndirim";

        // Kategori ve Çapraz Kampanya Destek Alanları
        public Guid? KategoriId { get; set; }
        public Guid? TetikleyiciKategoriId { get; set; }
        public Guid? HedefKategoriId { get; set; }
        public List<Guid> HedefUrunIds { get; set; } = new List<Guid>();
        public bool KuponlarlaBirlesebilirMi { get; set; }
        public int Sira { get; set; }
        public bool AktifMi { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
    }
}