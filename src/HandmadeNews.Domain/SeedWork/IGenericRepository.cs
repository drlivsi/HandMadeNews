using System.Linq.Expressions;

namespace HandmadeNews.Domain.SeedWork
{
    public interface IGenericRepository<T> : IDisposable where T : class
    {
        Task<IEnumerable<T>> FindBy(Expression<Func<T, bool>> predicate);
        Task Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<int> Count(Expression<Func<T, bool>> predicate);
        IQueryable<T> Set();
    }
}