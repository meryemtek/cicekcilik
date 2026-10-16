using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Siparis;

public class SiparisDurumGuncelleDto
{
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Lütfen sipariş durumunu seçin.")]
    [EnumDataType(typeof(SiparisDurumu), ErrorMessage = "Geçerli bir sipariş durumu seçin.")]
    public SiparisDurumu? YeniDurum { get; set; }
}
