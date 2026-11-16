using System.Linq.Expressions;
using Hrms.Domain.Entities;

namespace Hrms.Application.Interfaces;

public interface IHrmsRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> Query(params Expression<Func<TEntity, object>>[] includes);

    Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default);

    void Add(TEntity entity);

    void Remove(TEntity entity);

    Task SaveChangesAsync(CancellationToken ct = default);
}
