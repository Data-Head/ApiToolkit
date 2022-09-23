using System.Linq.Expressions;
using ApiToolkit.Models;
using ApiToolkit.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ApiToolkit.Repository;

public class GenericRepository<T, TJ> : IGenericRepository<T> where T : class where TJ : DbContext
{
    private readonly TJ _context;
    private readonly DbSet<T> _set;

    public GenericRepository(TJ context, DbSet<T> set)
    {
        _context = context;
        _set = set;
    }

    public async Task<IEnumerable<T>> GetAll<TKey>(Expression<Func<T, TKey>> orderBy, bool ascending = true,
        CancellationToken cancellationToken = default)
    {
        List<T> result;

        if (ascending)
        {
            result = await _set.OrderBy(orderBy).ToListAsync(cancellationToken);
        }
        else
        {
            result = await _set.OrderByDescending(orderBy).ToListAsync(cancellationToken);
        }

        return result;
    }

    public async Task<PaginatedData<T>> GetAllPaginated<TKey>(Expression<Func<T, TKey>> orderBy, bool ascending = true,
        int page = 1, int countPerPage = 20,
        CancellationToken cancellationToken = default)
    {
        List<T> result;

        var totalRecords = await _set.CountAsync(cancellationToken);

        if (ascending)
        {
            result = await _set.Skip((page - 1) * countPerPage).Take(countPerPage).OrderBy(orderBy)
                .ToListAsync(cancellationToken);
        }
        else
        {
            result = await _set.Skip((page - 1) * countPerPage).Take(countPerPage).OrderByDescending(orderBy)
                .ToListAsync(cancellationToken);
        }

        return new PaginatedData<T>()
        {
            Page = page,
            PageCount = (int) Math.Ceiling(totalRecords / (decimal) countPerPage),
            TotalRecords = totalRecords,
            RecordsPerPage = countPerPage,
            Data = result
        };
    }

    public async Task<T?> GetById(object?[]? keyValues, CancellationToken cancellationToken = default)
    {
        var result = await _set.FindAsync(keyValues, cancellationToken: cancellationToken);
        return result;
    }

    public async Task<T?> GetById(long id, CancellationToken cancellationToken = default)
    {
        var result = await _set.FindAsync(new object?[] {id}, cancellationToken: cancellationToken);

        return result;
    }

    public async Task<IEnumerable<T>> Where(Expression<Func<T, bool>> expression,
        CancellationToken cancellationToken = default)
    {
        var result = await _set.Where(expression).ToListAsync(cancellationToken);
        return result;
    }

    public async Task<PaginatedData<T>> WherePaginated<TKey>(Expression<Func<T, bool>> expression, int page = 1,
        int countPerPage = 20, Expression<Func<T, TKey>>? orderBy = null,
        bool ascending = true, CancellationToken cancellationToken = default)
    {
        var resultQuery = _set.Where(expression).Skip((page - 1) * countPerPage).Take(countPerPage);
        var totalRecords = await _set.Where(expression).CountAsync(cancellationToken);

        List<T> result;
        if (orderBy == null)
        {
            result = await resultQuery.ToListAsync(cancellationToken);
        }
        else
        {
            result = await (ascending
                ? resultQuery.OrderBy(orderBy).ToListAsync(cancellationToken)
                : resultQuery.OrderByDescending(orderBy).ToListAsync(cancellationToken));
        }

        return new PaginatedData<T>()
        {
            Page = page,
            PageCount = (int) Math.Ceiling(totalRecords / (decimal) countPerPage),
            TotalRecords = totalRecords,
            RecordsPerPage = countPerPage,
            Data = result
        };
    }

    public async Task<T?> WhereSingle(Expression<Func<T, bool>> expression,
        CancellationToken cancellationToken = default)
    {
        var result = await _set.Where(expression).FirstOrDefaultAsync(cancellationToken);
        return result;
    }

    public async Task InsertOrUpdate(T obj, CancellationToken cancellationToken = default)
    {
        var isTracked = _context.Entry(obj).State != EntityState.Detached;

        if (!isTracked)
        {
            if (obj is BaseModel model)
            {
                model.CreatedAt = DateTime.UtcNow;
                model.UpdatedAt = DateTime.UtcNow;
            }

            _set.Add(obj);
        }
        else
        {
            if (obj is BaseModel model)
            {
                model.UpdatedAt = DateTime.UtcNow;
            }

            _set.Update(obj);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task Delete(T obj, CancellationToken cancellationToken = default)
    {
        _set.Remove(obj);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<T> Queryable()
    {
        return _set.AsQueryable();
    }
}