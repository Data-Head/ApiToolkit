using System.Linq.Expressions;
using ApiToolkit.ViewModels;

namespace ApiToolkit.Repository;

public interface IGenericRepository<T>
{
    Task<IEnumerable<T>> GetAll<TKey>(Expression<Func<T, TKey>> orderBy, bool ascending = true,
        CancellationToken cancellationToken = default);

    Task<PaginatedData<T>> GetAllPaginated<TKey>(Expression<Func<T, TKey>> orderBy, bool ascending = true, int page = 1,
        int countPerPage = 20, CancellationToken cancellationToken = default);

    Task<T?> GetById(object?[]? keyValues, CancellationToken cancellationToken = default);

    Task<T?> GetById(long id, CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> Where(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);

    Task<PaginatedData<T>> WherePaginated<TKey>(Expression<Func<T, bool>> expression, int page = 1,
        int countPerPage = 20, Expression<Func<T, TKey>>? orderBy = null, bool ascending = true,
        CancellationToken cancellationToken = default);

    Task<T?> WhereSingle(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);

    Task InsertOrUpdate(T obj, CancellationToken cancellationToken = default);

    Task Delete(T obj, CancellationToken cancellationToken = default);

    IQueryable<T> Queryable();
}