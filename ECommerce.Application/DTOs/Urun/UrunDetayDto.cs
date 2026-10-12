using ECommerce.Application.DTOs.UrunVaryant;
using ECommerce.Application.DTOs.UrunYorum;

namespace ECommerce.Application.DTOs.Urun
{
    public class UrunDetayDto
    {
        public Guid Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public string? UrunKodu { get; set; }
        public string SeoUrl { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public string AiAciklama { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public int StokMiktari { get; set; }
        public string GorselUrl { get; set; } = string.Empty;
        public string? Etiket { get; set; }
        public Guid KategoriId { get; set; }
        public string KategoriAdi { get; set; } = string.Empty; // Kategori ismi için

        // İlgili Alt Listeler
        public List<UrunVaryantListeDto> Varyantlar { get; set; } = new();
        public List<UrunYorumListeDto> Yorumlar { get; set; } = new();

        // Hesaplanan Değerler
        public double OrtalamaPuan { get; set; }
        public int ToplamYorumSayisi { get; set; }
    }
}