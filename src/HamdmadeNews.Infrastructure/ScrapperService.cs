using HandmadeNews.Domain;
using HtmlAgilityPack;
using Telegram.Bot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using HamdmadeNews.Infrastructure.Scrapers;
using Microsoft.Extensions.Options;
using HandmadeNews.AzureFunc.Options;
using Microsoft.Extensions.DependencyInjection;
using HamdmadeNews.Infrastructure.Parsers;

namespace HamdmadeNews.Infrastructure
{
    public class ScrapperService : IScrapperService
    {        
        private readonly IOptions<ProducersOptions> _producersOptions;
        private readonly IServiceProvider _serviceProvider;
        private readonly IUnitOfWork _unitOfWork;

        public ScrapperService(IServiceProvider serviceProvider, IUnitOfWork unitOfWork, IOptions<ProducersOptions> producersOptions)
        {         
            _serviceProvider = serviceProvider;
            _unitOfWork = unitOfWork;
            _producersOptions = producersOptions;
        }

        public async Task DoScrap()
        {
            var tasks = new List<Task<List<Article>>>();            

            var lanarteParser = (IParser)_serviceProvider.GetRequiredService(typeof(LanarteParser));
            tasks.Add(lanarteParser.Parse(_producersOptions.Value.LanarteUrl));

            var bucillaParser = (IParser)_serviceProvider.GetRequiredService(typeof(BucillaParser));
            tasks.Add(bucillaParser.Parse(_producersOptions.Value.BucillaUrl));

            await Task.WhenAll(tasks);          

            foreach(var task in tasks)
            {
                task.Result.ForEach(async x =>
                {
                    var existingArticle = await GetArticle(x.Producer, x.Code);
                    if (existingArticle == null)
                    {
                        await AddArticle(x);
                    }                        
                });
            }
        }

        public async Task SendToTelegram()
        {
            string apiToken = "5933325897:AAF0Q79hZp457OEeDKuQpp89bBdRC_zKDAA";
            string chatIdRu = "-1001682303552";
            string chatIdUa = "-1001890605319";

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
