namespace ECommerce.Application.DTOs.Siparis;

public class SiparisDurumGuncelleSonucDto
{
    public bool DurumDegisti { get; set; }
    // null: Bu işlem için SMS gönderimi gerekmedi. true: Netgsm gönderim isteğini kabul etti.
    public bool? SmsKabulEdildi { get; set; }
    // null: Müşteri e-postası yok veya bu durum için e-posta gönderilmedi.
    public bool? EmailKabulEdildi { get; set; }
    public string Mesaj { get; set; } = string.Empty;
}
