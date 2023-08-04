using HandmadeNews.Domain.Entities;

namespace HandmadeNews.Infrastructure.Parsing.Strategies
{
    public interface IParsingStrategy
    {
        List<Article> Parse(string html, string domain);
    }
}