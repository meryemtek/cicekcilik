using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Auth;

public class SifreSifirlaDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    public string YeniSifre { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(YeniSifre), ErrorMessage = "Şifreler birbiriyle eşleşmiyor.")]
    public string YeniSifreTekrar { get; set; } = string.Empty;
}
