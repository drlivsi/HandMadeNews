namespace HamdmadeNews.Infrastructure.Services
{
    public interface IScrapperService
    {
        Task DoScrap();
        Task SendToTelegram();
    }
}
