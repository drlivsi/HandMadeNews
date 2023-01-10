using HandmadeNews.Domain;
using HtmlAgilityPack;
using System.Text;
using System;
using System.Web;

namespace HamdmadeNews.Infrastructure.Parsers.Producers
{
    public class BucillaParser : BaseParser
    {
        public override async Task<List<Article>> Parse(string pageUrl)
        {
            var articles = new List<Article>();

            var html = await CallUrl(pageUrl);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            foreach (var item in htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'product-tile')]"))
            {
                var article = new Article()
                {
                    Producer = HandmadeNews.Domain.Producers.Bucilla
                };

                var a = item.Descendants("a").FirstOrDefault(m => string.IsNullOrEmpty(m.GetAttributeValue("tile-link", "")));


                article.Title = HttpUtility.HtmlDecode(a.GetAttributeValue("title", "").Replace("Bucilla &#174; ", string.Empty));
                article.Url = "https://plaidonline.com" + a.GetAttributeValue("href", "");
                article.Code = article.Url.Split("-").Last();

                var image = item.Descendants("img").FirstOrDefault();
                article.Img = "https://plaidonline.com" + image.GetAttributeValue("src", "");

                articles.Add(article);
            }

            return articles;
        }
    }
}
