using HandmadeNews.Domain.Entities;

namespace HamdmadeNews.Infrastructure.Parsing.Strategies
{
    public interface IParsingStrategy
    {
        List<Article> Parse(string html, string domain);
    }
}