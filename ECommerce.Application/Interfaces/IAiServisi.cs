namespace ECommerce.Application.Interfaces;

public interface IAiServisi
{
    Task<string> UrunAciklamasiUretAsync(string urunAdi, string kategoriAdi);
}