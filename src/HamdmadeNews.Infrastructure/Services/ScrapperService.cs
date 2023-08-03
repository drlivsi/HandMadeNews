using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using HandmadeNews.AzureFunc.Options;
using HamdmadeNews.Infrastructure.Options;
using HamdmadeNews.Infrastructure.Parsing;
using HamdmadeNews.Infrastructure.Parsing.Strategies;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using HandmadeNews.Domain.Entities;
using HandmadeNews.Domain.Enums;
using HandmadeNews.Domain.SeedWork;

namespace HamdmadeNews.Infrastructure.Services
{
    public class ScrapperService : IScrapperService
    {
        private readonly IOptions<ProducersOptions> _producersOptions;
        private readonly IOptions<TelegramOptions> _telegramOptions;
        private readonly IServiceProvider _serviceProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IParser _parser;
        private readonly HttpClient _httpClient;

        public ScrapperService(
            IServiceProvider serviceProvider,
            IUnitOfWork unitOfWork,
            IParser parser,
            HttpClient httpClient,
            IOptions<TelegramOptions> telegramOptions,
            IOptions<ProducersOptions> producersOptions)
        {
            _serviceProvider = serviceProvider;
            _unitOfWork = unitOfWork;
            _parser = parser;
            _httpClient = httpClient;
            _telegramOptions = telegramOptions;
            _producersOptions = producersOptions;
        }

        public async Task DoScrap()
        {
            // Download html pages in parallel
            var tasks = new List<Task<(Type parserType, string html, string domain)>>();
            tasks.Add(DownloadHtml(typeof(LanarteParsingStrategy), _producersOptions.Value.LanarteUrl));
            tasks.Add(DownloadHtml(typeof(BucillaParsingStrategy), _producersOptions.Value.BucillaUrl));
            tasks.Add(DownloadHtml(typeof(KoolerdesignParsingStrategy), _producersOptions.Value.KoolerDesignUrl));
            var taskResults = await Task.WhenAll(tasks);

            // Process each html
            foreach (var taskResult in taskResults)
            {
                // Switch parsing strategy on the fly
                var strategy = (IParsingStrategy)_serviceProvider.GetRequiredService(taskResult.parserType);
                var articles = _parser.Process(strategy, taskResult.html, taskResult.domain);

                // add article to the database if not exists
                foreach (var article in articles)
                {
                    var existingArticle = await GetArticle(article.Producer, article.Code);
                    if (existingArticle == null)
                    {
                        await _unitOfWork.ArticlesRepository.Add(article);
                    }
                }
            }

            _unitOfWork.Save();
        }

        private async Task<(Type parsingStrategy, string html, string domain)> DownloadHtml(Type parserType, string url)
        {
            var html = await _httpClient.GetStringAsync(url);
            var uri = new Uri(url);
            var domain = uri.Scheme + "://" + uri.Host;
            return (parserType, html, domain);
        }

        public async Task SendToTelegram()
        {
            string apiToken = _telegramOptions.Value.ApiKey;
            string chatIdRu = _telegramOptions.Value.ChatIdRu;
            string chatIdUa = _telegramOptions.Value.ChatIdUa;

            var botClient = new TelegramBotClient(apiToken);

            var articles = await GetArticlesNotInTelegram();

            int processed = 0;

            foreach (var article in articles)
            {
                if (!article.TelegramRu)
                {
                    await botClient.SendPhotoAsync(
                        chatId: chatIdRu,
                        photo: article.Img,
                        caption: $"<b><i><a href=\"{article.Url}\">{article.Producer}: {article.Title}</a></i></b>",
                        parseMode: ParseMode.Html);

                    article.TelegramRu = true;
                    UpdateArticle(article);
                    processed++;
                }

                if (!article.TelegramUa)
                {
                    await botClient.SendPhotoAsync(
                        chatId: chatIdUa,
                        photo: article.Img,
                        caption: $"<b><i><a href=\"{article.Url}\">{article.Producer}: {article.Title}</a></i></b>",
                        parseMode: ParseMode.Html);

                    article.TelegramUa = true;
                    UpdateArticle(article);
                    processed++;
                }

                // Send tomorrow
                if (processed >= 20)
                    break;
            };
        }

        protected async Task<List<Article>> GetArticlesNotInTelegram()
        {
            return (await _unitOfWork.ArticlesRepository.FindBy(m => m.TelegramRu == false || m.TelegramUa == false)).ToList();
        }

        protected void UpdateArticle(Article article)
        {
            _unitOfWork.ArticlesRepository.Update(article);
            _unitOfWork.Save();
        }

        protected async Task<Article?> GetArticle(Producers producer, string code)
        {
            return (await _unitOfWork.ArticlesRepository.FindBy(m => m.Producer == producer && m.Code == code)).FirstOrDefault();
        }

        protected Task AddArticle(Article article)
        {
            _unitOfWork.ArticlesRepository.Add(article);
            _unitOfWork.Save();
            return Task.CompletedTask;
        }
    }
}
