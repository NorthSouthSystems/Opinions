using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace NorthSouthSystems.Entities;

public sealed record DbEntitySet<TEntity>(DbContext DbContext) : IUnitOfWorkEntitySet<TEntity>, IAsyncEnumerable<TEntity>
    where TEntity : class
{
    // DbSet

    private DbSet<TEntity> DbSet => DbContext.Set<TEntity>();

    public ICollection<TEntity> Local => DbSet.Local;
    IReadOnlyCollection<TEntity> IReadOnlyEntitySet<TEntity>.Local => Local.AsReadOnlyCollectionWrapper();

    public void Add(TEntity entity) => DbSet.Add(entity);
    public void Update(TEntity entity) => DbSet.Update(entity);
    public void Remove(TEntity entity) => DbSet.Remove(entity);

    // IQueryable

    private IQueryable<TEntity> AsQueryable => DbSet;

    Type IQueryable.ElementType => AsQueryable.ElementType;
    Expression IQueryable.Expression => AsQueryable.Expression;
    IQueryProvider IQueryable.Provider => AsQueryable.Provider;

    IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator() => AsQueryable.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => AsQueryable.GetEnumerator();

    // IAsyncEnumerable
    //
    // Without it, Entity Framework Core throws the following Exception for every Async operation:
    // System.InvalidOperationException: The source 'IQueryable' doesn't implement 'IAsyncEnumerable<...>'.
    // Only sources that implement 'IAsyncEnumerable' can be used for Entity Framework asynchronous operations.

    IAsyncEnumerator<TEntity> IAsyncEnumerable<TEntity>.GetAsyncEnumerator(CancellationToken cancellationToken) =>
        DbSet.AsAsyncEnumerable().GetAsyncEnumerator(cancellationToken);
}