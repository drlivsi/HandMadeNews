using HandmadeNews.Domain;

namespace HamdmadeNews.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Article> ArticlesRepository { get; }
        int Save();
    }
}