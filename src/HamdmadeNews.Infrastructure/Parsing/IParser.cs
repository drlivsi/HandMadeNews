using HamdmadeNews.Infrastructure.Parsing.Strategies;
using HandmadeNews.Domain.Entities;

namespace HamdmadeNews.Infrastructure.Parsing;

public interface IParser
{
    List<Article> Process(IParsingStrategy strategy, string html, string domain);
}