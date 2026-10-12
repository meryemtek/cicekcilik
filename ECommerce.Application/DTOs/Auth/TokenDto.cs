namespace ECommerce.Application.DTOs.Auth;

public class TokenDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime BitisTarihi { get; set; }
    public string Mesaj { get; set; } = string.Empty;
}