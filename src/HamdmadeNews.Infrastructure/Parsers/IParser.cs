using HandmadeNews.Domain;

namespace HamdmadeNews.Infrastructure.Scrapers
{
    public interface IParser
    {
        Task<List<Article>> Parse(string pageUrl);
    }
}