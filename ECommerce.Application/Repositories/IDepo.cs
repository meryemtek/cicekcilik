using System.Linq.Expressions;

namespace ECommerce.Application.Repositories;

public interface IDepo<T> where T : class
{
    Task<IReadOnlyList<T>> TumunuGetirAsync();
    Task<IReadOnlyList<T>> GetirAsync(Expression<Func<T, bool>> kosul);
    Task<T?> IdIleGetirAsync(Guid id);
    Task EkleAsync(T varlik);
    void Guncelle(T varlik);
    void Sil(T varlik);
    Task<int> DegisiklikleriKaydetAsync();
}