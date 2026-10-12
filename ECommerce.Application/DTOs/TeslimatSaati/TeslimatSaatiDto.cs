using ECommerce.Domain.Enums;

namespace ECommerce.Application.DTOs.TeslimatSaati
{
    public class TeslimatSaatiDto
    {
        public Guid Id { get; set; }
        public string BaslangicSaati { get; set; }
        public string BitisSaati { get; set; }
        public string Aciklama { get; set; }
        public int GunlukKota { get; set; }
        public int Sira { get; set; }
        public bool AktifMi { get; set; }

        // YENİ: Normal mi VIP mi olduğunu tutar (1: Normal, 2: Vip)
        public TeslimatGrubu TeslimatGrubu { get; set; }
    }
}