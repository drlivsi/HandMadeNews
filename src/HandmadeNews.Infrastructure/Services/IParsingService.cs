namespace HandmadeNews.Infrastructure.Services
{
    public interface IParsingService
    {
        Task Parse();
        Task SendToTelegram();
    }
}
