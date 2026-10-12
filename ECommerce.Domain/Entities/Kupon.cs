namespace ECommerce.Domain.Entities;

public class Kupon : TemelVarlik
{
    public string Kod { get; set; } = string.Empty;
    public decimal IndirimMiktari { get; set; }
    public bool YuzdelikMi { get; set; } // True ise %20 indirim, False ise 50 TL indirim gibi
    public DateTime BitisTarihi { get; set; }
    public bool AktifMi { get; set; } = true;
}