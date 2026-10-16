namespace ECommerce.Application.DTOs.Duyuru
{
    public class DuyuruEkleDto
    {
        public string Metin { get; set; }
        public string? LinkUrl { get; set; }
        public int SiraNo { get; set; }
        public bool AktifMi { get; set; } = true;
    }
}
