namespace NorthSouthSystems.Entities;

public interface IReadOnlyEntitySet<TEntity> : IQueryable<TEntity>
{
    IReadOnlyCollection<TEntity> Local { get; }
}

public interface IUnitOfWorkEntitySet<TEntity> : IReadOnlyEntitySet<TEntity>
{
    new ICollection<TEntity> Local { get; }

    void Add(TEntity entity);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}