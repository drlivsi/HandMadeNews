using HandmadeNews.Infrastructure.Parsing.Strategies;
using HandmadeNews.Domain.Entities;

namespace HandmadeNews.Infrastructure.Parsing;

public interface IParser
{
    List<Article> Process(IParsingStrategy strategy, string html, string domain);
}