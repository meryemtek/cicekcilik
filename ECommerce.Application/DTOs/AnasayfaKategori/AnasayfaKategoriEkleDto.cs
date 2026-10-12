namespace ECommerce.Application.DTOs.AnasayfaKategori
{
    public class AnasayfaKategoriEkleDto
    {
        public Guid KategoriId { get; set; }
        public int SiraNo { get; set; }
        public bool AktifMi { get; set; } = true;
    }
}