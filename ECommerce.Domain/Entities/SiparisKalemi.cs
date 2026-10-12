using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities;

public class SiparisKalemi : TemelVarlik
{
    public Guid SiparisId { get; set; }
    public virtual Siparis Siparis { get; set; } = null!;
    public Guid UrunId { get; set; }
    public virtual Urun Urun { get; set; } = null!;
    public Guid? VaryantId { get; set; }
    public string VaryantAdi { get; set; } = string.Empty;
    public int Miktar { get; set; }
    public decimal BirimFiyat { get; set; }
    public decimal ToplamFiyat => BirimFiyat * Miktar;
    public string GorselUrl { get; set; } = string.Empty;

    // Sepette her kaleme özel atanan operasyonel teslimat bölge ve zaman bilgileri
    public string Il { get; set; } = string.Empty;
    public string Ilce { get; set; } = string.Empty;
    public string Mahalle { get; set; } = string.Empty;
    public string TeslimatGunu { get; set; } = string.Empty;
    public string TeslimatSaati { get; set; } = string.Empty;
    public bool VipTeslimatAktifMi { get; set; } = false;
    public decimal TeslimatUcreti { get; set; }
    public decimal VipUcreti { get; set; }
}