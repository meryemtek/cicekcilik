namespace ECommerce.Application.DTOs.AnasayfaKategori
{
    public class AnasayfaKategoriGuncelleDto
    {
        public Guid Id { get; set; }
        public Guid KategoriId { get; set; }
        public int SiraNo { get; set; }
        public bool AktifMi { get; set; }
        public string? GorselUrl { get; set; }
    }
}
