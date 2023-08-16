using HandmadeNews.Domain.Entities;
using HandmadeNews.Infrastructure.Models;

namespace HandmadeNews.Infrastructure.Services
{
    public interface IParsingService
    {
        Task<ParsedItem[]> DownloadAsync(Dictionary<Type, string> producers);
        List<Article> Parse(ParsedItem[] parsedItems);
        Task SaveIfNewAsync(List<Article> articles);
        Task SendToTelegram();
    }
}
