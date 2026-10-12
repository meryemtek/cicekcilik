using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class Duyuru
    {
        public Guid Id { get; set; }
        public string Metin { get; set; }        // Örn: İLK ÜYELİĞE ÖZEL 100TL İNDİRİM!
        public string LinkUrl { get; set; }     // İsteğe bağlı tıklanınca gidilecek adres
        public int SiraNo { get; set; }
        public bool AktifMi { get; set; } = true;
    }
}
