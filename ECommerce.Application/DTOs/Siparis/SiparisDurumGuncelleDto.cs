using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;

namespace ECommerce.Application.DTOs.Siparis;

public class SiparisDurumGuncelleDto
{
    public Guid Id { get; set; }
    public SiparisDurumu YeniDurum { get; set; }
}