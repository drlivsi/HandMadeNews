using HamdmadeNews.Infrastructure.Data;
using HandmadeNews.Domain;

namespace HamdmadeNews.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _context;

        public IGenericRepository<Article> ArticlesRepository { get; }


        public UnitOfWork(ApplicationDbContext context, IGenericRepository<Article> articlesRepository)
        {
            _context = context;
            ArticlesRepository = articlesRepository;
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
                _context = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}