namespace ECommerce.Domain.Entities;

public class SikSorulanSoru : TemelVarlik
{
    public string Soru { get; set; } = string.Empty;
    public string Cevap { get; set; } = string.Empty;
    public int SiraNo { get; set; }
    public bool AktifMi { get; set; } = true;
}