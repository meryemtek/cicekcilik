using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;

namespace ECommerce.Application.DTOs.Siparis;

public class SiparisListeDto
{
    public Guid Id { get; set; }
    public string SiparisNumarasi { get; set; } = string.Empty;
    public DateTime SiparisTarihi { get; set; }
    public decimal ToplamTutar { get; set; }
    public SiparisDurumu Durum { get; set; }
    public string MusteriAdSoyad { get; set; } = string.Empty;
}