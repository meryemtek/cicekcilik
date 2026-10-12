namespace ECommerce.Application.DTOs.Hakkimizda
{
    public class HakkimizdaAyariDto
    {
        public bool HikayemizAktifMi { get; set; } = true;
        public string HikayemizBaslik { get; set; } = "Hikayemiz";
        public string HikayemizMetni { get; set; } = "";
        public string? GorselUrl { get; set; } = ""; // YENİ: Hakkımızda Görseli

        public bool NedenBiziAktifMi { get; set; } = true;
        public List<NedenBiziItemDto> NedenBiziItems { get; set; } = new();

        public bool DegerlerimizAktifMi { get; set; } = true;
        public List<DegerItemDto> Degerlerimiz { get; set; } = new();

        public bool HizmetBolgeleriAktifMi { get; set; } = true;
        public List<string> HizmetBolgeleri { get; set; } = new();

        public string SeoTitle { get; set; } = "";
        public string SeoDescription { get; set; } = "";
    }
    // (Diğer alt DTO'lar aynı kalacak)
}