namespace ECommerce.Application.DTOs.AnasayfaKategori
{
    public class AnasayfaKategoriDto
    {
        public Guid Id { get; set; }
        public Guid KategoriId { get; set; }
        public string KategoriAdi { get; set; } = string.Empty;
        public string SeoUrl { get; set; } = string.Empty;
        public string GorselUrl { get; set; } = string.Empty;
        public string? VitrinGorselUrl { get; set; }
        public string? Aciklama { get; set; }
        public int SiraNo { get; set; }
        public bool AktifMi { get; set; }
    }
}
