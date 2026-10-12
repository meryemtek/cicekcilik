using System;

namespace ECommerce.Application.DTOs.Duyuru
{
    public class DuyuruDto
    {
        public Guid Id { get; set; }
        public string Metin { get; set; }
        public string LinkUrl { get; set; }
        public int SiraNo { get; set; }
        public bool AktifMi { get; set; }
    }
}