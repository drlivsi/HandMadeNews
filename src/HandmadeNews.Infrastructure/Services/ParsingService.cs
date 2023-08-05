using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using HandmadeNews.Infrastructure.Options;
using HandmadeNews.Infrastructure.Parsing;
using HandmadeNews.Infrastructure.Parsing.Strategies;
using HandmadeNews.Domain.Entities;
using HandmadeNews.Domain.Enums;
using HandmadeNews.Domain.SeedWork;
using Microsoft.Extensions.Logging;

namespace HandmadeNews.Infrastructure.Services
{
    public class ParsingService : IParsingService
    {
        private readonly IOptions<ProducersOptions> _producersOptions;
        private readonly IOptions<TelegramOptions> _telegramOptions;
        private readonly ILogger<ParsingService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IParser _parser;
        private readonly HttpClient _httpClient;

        public ParsingService(
            ILogger<ParsingService> logger,
            IServiceProvider serviceProvider,
            IUnitOfWork unitOfWork,
            IParser parser,
            HttpClient httpClient,
            IOptions<TelegramOptions> telegramOptions,
            IOptions<ProducersOptions> producersOptions)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _unitOfWork = unitOfWork;
            _parser = parser;
            _httpClient = httpClient;
            _telegramOptions = telegramOptions;
            _producersOptions = producersOptions;
        }

        public async Task Parse()
        {
            // Download html pages in parallel
            var tasks = new List<Task<(Type parserType, string html, string domain)>>
            {
                DownloadHtml(typeof(LanarteParsingStrategy), _producersOptions.Value.LanarteUrl),
                DownloadHtml(typeof(BucillaParsingStrategy), _producersOptions.Value.BucillaUrl),
                DownloadHtml(typeof(KoolerdesignParsingStrategy), _producersOptions.Value.KoolerDesignUrl)
            };

            var aggregateTask = Task.WhenAll(tasks);

            (Type parserType, string html, string domain)[] allResults = null;

            try
            {
                allResults = await aggregateTask;
            }
            catch (Exception ex)
            {
                if (aggregateTask.Exception != null && aggregateTask.Exception.InnerExceptions.Any())
                {
                    foreach (var innerException in aggregateTask.Exception.InnerExceptions)
                    {
                        _logger.LogError(innerException, "Something is wrong");
                    }
                }
                else
                {
                    _logger.LogError(ex, "Something is wrong");
                }
            }

            if (allResults == null)
            {
                return;
            }

            // Process each html
            foreach (var taskResult in allResults)
            {
                if (string.IsNullOrEmpty(taskResult.html))
                    continue;

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

                _unitOfWork.Save();
            }
        }


        private async Task<(Type parsingStrategy, string html, string domain)> DownloadHtml(Type parserType, string url)
        {
            var uri = new Uri(url);
            var domain = uri.Scheme + "://" + uri.Host;
            string html = string.Empty;

            try
            { 
                html = await _httpClient.GetStringAsync(url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An attempt to download html failed: parserType={parserType},url={url} ");
            }

            return (parserType, html, domain);
        }

        public async Task SendToTelegram()
        {
            if(!_telegramOptions.Value.Enabled)
                return;

            var apiToken = _telegramOptions.Value.ApiKey;
            var chatIdRu = _telegramOptions.Value.ChatIdRu;
            var chatIdUa = _telegramOptions.Value.ChatIdUa;

            var botClient = new TelegramBotClient(apiToken);

            var articles = await GetArticlesNotInTelegram();

            int processed = 0;

            foreach (var article in articles)
            {
                // I prefer to send images one by one

                if (!article.TelegramRu)
                {
                    await botClient.SendPhotoAsync(
                        chatId: chatIdRu,
                        photo: article.Img,
                        caption: $"<b><i><a href=\"{article.Url}\">{article.Producer}: {article.Title}</a></i></b>",
                        parseMode: ParseMode.Html);

                    article.TelegramRu = true;
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
                    processed++;
                }

                // Send other images tomorrow
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
