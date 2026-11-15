using System.Linq.Expressions;
using Hrms.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hrms.Infrastructure.Data;

public class EfRepository<TEntity>(ApplicationDbContext context) : IHrmsRepository<TEntity> where TEntity : class
{
    protected readonly ApplicationDbContext Context = context;

    public IQueryable<TEntity> Query(params Expression<Func<TEntity, object>>[] includes) =>
        includes.Aggregate(Context.Set<TEntity>().AsQueryable(), (q, inc) => q.Include(inc));

    public Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default) =>
        Context.Set<TEntity>().FindAsync([id], ct).AsTask();

    public void Add(TEntity entity) => Context.Set<TEntity>().Add(entity);

    public void Remove(TEntity entity) => Context.Set<TEntity>().Remove(entity);

    public Task SaveChangesAsync(CancellationToken ct = default) => Context.SaveChangesAsync(ct);
}
