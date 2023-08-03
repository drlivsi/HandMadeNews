using HandmadeNews.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HamdmadeNews.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Article> Articles { get; set; }
    }
}
