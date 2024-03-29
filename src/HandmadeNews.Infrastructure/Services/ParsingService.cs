﻿using HandmadeNews.Domain.Entities;
using HandmadeNews.Domain.Enums;
using HandmadeNews.Domain.SeedWork;
using HandmadeNews.Infrastructure.Models;
using HandmadeNews.Infrastructure.Options;
using HandmadeNews.Infrastructure.Parsing;
using HandmadeNews.Infrastructure.Parsing.Strategies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace HandmadeNews.Infrastructure.Services
{
    public class ParsingService : IParsingService
    {
        private readonly IOptions<TelegramOptions> _telegramOptions;
        private readonly ILogger<ParsingService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IParser _parser;
        private readonly HttpClient _httpClient;

        public ParsingService(
            ILogger<ParsingService> logger,
            IOptions<TelegramOptions> telegramOptions,
            IServiceProvider serviceProvider,
            IUnitOfWork unitOfWork,
            IParser parser,
            HttpClient httpClient)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _unitOfWork = unitOfWork;
            _parser = parser;
            _httpClient = httpClient;
            _telegramOptions = telegramOptions;
        }

        public async Task<ParsedItem[]> DownloadAsync(Dictionary<Type, string> producers)
        {
            var results = new ConcurrentBag<ParsedItem>();

            var tasks = producers.Select(
                async producer =>
                {
                    try
                    {
                        var html = await _httpClient.GetStringAsync(producer.Value);
                        results.Add(new ParsedItem()
                        {
                            ProducerType = producer.Key,
                            Url = new Uri(producer.Value),
                            Html = html,
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"An attempt to download html failed: parserType={producer.Key},url={producer.Value} ");
                    }
                });
            
            await Task.WhenAll(tasks);
            
            return results.ToArray();
        }

        public List<Article> Parse(ParsedItem[] parsedItems)
        {
            var allArticles = new List<Article>();

            // Process each html
            foreach (var taskResult in parsedItems)
            {
                if (string.IsNullOrEmpty(taskResult.Html))
                    continue;

                // Switch parsing strategy on the fly
                var strategy = (IParsingStrategy)_serviceProvider.GetRequiredService(taskResult.ProducerType);
                var articles = _parser.Process(strategy, taskResult.Html, taskResult.Domain);

                allArticles.AddRange(articles);
            }

            return allArticles;
        }

        public async Task SaveIfNewAsync(List<Article> articles)
        {
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

        protected async Task<Article?> GetArticle(Producers producer, string code)
        {
            return (await _unitOfWork.ArticlesRepository.FindBy(m => m.Producer == producer && m.Code == code)).FirstOrDefault();
        }
    }
}
