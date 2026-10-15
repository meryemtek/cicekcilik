using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Auth;

public class SifreDegistirDto
{
    [Required]
    public string MevcutSifre { get; set; } = string.Empty;
    [Required]
    [MinLength(6)]
    public string YeniSifre { get; set; } = string.Empty;
    [Required]
    [Compare(nameof(YeniSifre))]
    public string YeniSifreTekrar { get; set; } = string.Empty;
}
