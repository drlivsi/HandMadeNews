using HandmadeNews.Domain;
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

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Article>().ToTable("Articles");
        //    base.OnModelCreating(modelBuilder);
        //}
    }
}
