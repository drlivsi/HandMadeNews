using Dapper;
using Dapper.Contrib.Extensions;
using HtmlAgilityPack;
using MySqlConnector;
using System.Data;
using System.Net;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace Scraper
{
    public class BaseScraper
    {
        public async Task DoScrap()
        {
            string url = "https://webshop.verachtert.be/en-us/lan-arte/embroidery-kits/?sort=PfsSeason_desc&count=13&viewMode=list";
            string response = await CallUrl(url);
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);

            var articles = await GetArticles();

            foreach (var item in htmlDoc.DocumentNode.SelectNodes("//*[@data-id]"))
            {
                var article = new Article()
                {
                    Producer = "Lanarte"
                };

                article.Id = item.GetAttributeValue("data-id", "");

                if (articles.Any(m => m.Id == article.Id))
                    continue;

                var a = item.Descendants("a").FirstOrDefault(m => string.IsNullOrEmpty(m.GetAttributeValue("hyp-thumbnail", "")));
                

                article.Url = "https://webshop.verachtert.be" + a.GetAttributeValue("href", "");
                var image = a.Descendants("img").FirstOrDefault(m => string.IsNullOrEmpty(m.GetAttributeValue("data-src-original", "")));
                article.Title = image.GetAttributeValue("title", "");
                article.Img = "https://webshop.verachtert.be/product/image/large/" + article.Id + "_1.jpg";
                await AddArticle(article);               
            }
        }

        public async Task SendToTelegram()
        {
            string apiToken = "5933325897:AAF0Q79hZp457OEeDKuQpp89bBdRC_zKDAA";
            string chatId = "-1001682303552";

            var botClient = new TelegramBotClient(apiToken);

            var articles = await GetArticles();

            foreach(var article in articles)
            {
                var message = await botClient.SendPhotoAsync(
                chatId: chatId,
                photo: article.Img,
                caption: $"<b>{article.Title}</b>. <i>Source</i>: <a href=\"{article.Url}\">{article.Producer}</a>",
                parseMode: ParseMode.Html);                
                await UpdateArticle(article);
            };            
        }

        private async Task<string> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            client.DefaultRequestHeaders.Accept.Clear();
            var response = client.GetStringAsync(fullUrl);
            return await response;
        }

        public async Task<List<Article>> GetArticles()
        {
            using (IDbConnection db = new MySqlConnection("server=168.119.169.136;uid=kr_hmnews_user;pwd=ypigQoS3TO;database=kr_hmnews"))
            {
                return (await db.QueryAsync<Article>("SELECT * FROM Article")).ToList();
            }
        }

        public async Task AddArticle(Article article)
        {
            using (IDbConnection db = new MySqlConnection("server=168.119.169.136;uid=kr_hmnews_user;pwd=ypigQoS3TO;database=kr_hmnews"))
            {
                await db.InsertAsync<Article>(article);
            }
        }

        public async Task UpdateArticle(Article article)
        {
            using (IDbConnection db = new MySqlConnection("server=168.119.169.136;uid=kr_hmnews_user;pwd=ypigQoS3TO;database=kr_hmnews"))
            {
                await db.UpdateAsync<Article>(article);
            }
        }
    }
}