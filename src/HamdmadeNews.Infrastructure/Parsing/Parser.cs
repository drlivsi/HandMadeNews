using HamdmadeNews.Infrastructure.Parsing.Strategies;
using HandmadeNews.Domain.Entities;

namespace HamdmadeNews.Infrastructure.Parsing
{
    public class Parser : IParser
    {
        public List<Article> Process(IParsingStrategy strategy, string html, string domain)
        {
            return strategy.Parse(html, domain);
        }
    }
}