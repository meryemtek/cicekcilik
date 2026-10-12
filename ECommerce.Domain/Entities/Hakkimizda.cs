namespace ECommerce.Domain.Entities
{
    public class Hakkimizda : TemelVarlik
    {
        public bool HikayemizAktifMi { get; set; }
        public string HikayemizBaslik { get; set; } = string.Empty;
        public string HikayemizMetni { get; set; } = string.Empty;
        public string? GorselUrl { get; set; } = string.Empty; // YENİ

        public bool NedenBiziAktifMi { get; set; }
        public string NedenBiziJson { get; set; } = "[]";

        public bool DegerlerimizAktifMi { get; set; }
        public string DegerlerimizJson { get; set; } = "[]";

        public bool HizmetBolgeleriAktifMi { get; set; }
        public string HizmetBolgeleriJson { get; set; } = "[]";

        public string SeoTitle { get; set; } = string.Empty;
        public string SeoDescription { get; set; } = string.Empty;
    }
}