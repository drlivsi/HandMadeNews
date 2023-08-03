using HandmadeNews.Domain.Entities;
using HandmadeNews.Domain.Enums;
using HtmlAgilityPack;
using System.Web;

namespace HamdmadeNews.Infrastructure.Parsing.Strategies
{
    public class BucillaParsingStrategy : IParsingStrategy
    {
        public List<Article> Parse(string html, string domain)
        {
            var articles = new List<Article>();

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            foreach (var item in htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'product-tile')]"))
            {
                var article = new Article()
                {
                    Producer = Producers.Bucilla
                };

                var tagA = item.Descendants("a").FirstOrDefault(m => string.IsNullOrEmpty(m.GetAttributeValue("tile-link", "")));
                if (tagA == null)
                {
                    // TODO: add some warning if html tag is null
                    continue;
                }

                article.Title = HttpUtility.HtmlDecode(tagA.GetAttributeValue("title", "").Replace("Bucilla &#174; ", string.Empty));
                article.Url = domain + tagA.GetAttributeValue("href", "");
                article.Code = article.Url.Split("-").Last();

                var image = item.Descendants("img").FirstOrDefault();

                if (image == null)
                {
                    // TODO: add some warning if html tag is null
                    continue;
                }

                article.Img = domain + image.GetAttributeValue("src", "");

                articles.Add(article);
            }

            return articles;
        }
    }
}
