namespace ECommerce.Application.Interfaces;

public interface IEmailServisi
{
    Task<bool> EpostaGonderAsync(string aliciEmail, string konu, string htmlIcerik);
}
