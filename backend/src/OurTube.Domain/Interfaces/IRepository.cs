using System.Linq.Expressions;

namespace OurTube.Domain.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity? Get(params object[] keyValues);
        IQueryable<TEntity> GetAll();
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entity);

        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);

        bool Contains(params object[] keyValues);
    }
}
