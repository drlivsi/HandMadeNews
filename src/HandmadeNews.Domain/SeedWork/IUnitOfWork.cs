using HandmadeNews.Domain.Entities;

namespace HandmadeNews.Domain.SeedWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Article> ArticlesRepository { get; }
        int Save();
    }
}