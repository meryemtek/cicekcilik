using System.Linq.Expressions;
using ECommerce.Application.Repositories;
using ECommerce.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class Depo<T> : IDepo<T> where T : class
{
    protected readonly UygulamaDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Depo(UygulamaDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<IReadOnlyList<T>> TumunuGetirAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IReadOnlyList<T>> GetirAsync(Expression<Func<T, bool>> kosul)
    {
        return await _dbSet.Where(kosul).ToListAsync();
    }

    public async Task<T?> IdIleGetirAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<T?> IdIleGetirAsync(Guid id, Func<IQueryable<T>, IQueryable<T>>? include = null)
    {
        IQueryable<T> query = _dbSet;

        if (include != null)
        {
            query = include(query);
        }

        var keyName = _context.Model.FindEntityType(typeof(T))?
            .FindPrimaryKey()?
            .Properties
            .Select(p => p.Name)
            .FirstOrDefault() ?? "Id";

        var parameter = Expression.Parameter(typeof(T), "e");
        var property = Expression.Property(parameter, keyName);
        var constant = Expression.Constant(id, property.Type);
        var equals = Expression.Equal(property, constant);
        var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);

        return await query.FirstOrDefaultAsync(lambda);
    }

    public async Task EkleAsync(T varlik)
    {
        await _dbSet.AddAsync(varlik);
    }

    public void Guncelle(T varlik)
    {
        _dbSet.Update(varlik);
    }

    public void Sil(T varlik)
    {
        _dbSet.Remove(varlik);
    }

    public async Task<int> DegisiklikleriKaydetAsync()
    {
        return await _context.SaveChangesAsync();
    }
}