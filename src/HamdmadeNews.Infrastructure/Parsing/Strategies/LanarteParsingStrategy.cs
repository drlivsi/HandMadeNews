using HandmadeNews.Domain.Entities;
using HandmadeNews.Domain.Enums;
using HtmlAgilityPack;

namespace HamdmadeNews.Infrastructure.Parsing.Strategies
{
    public class LanarteParsingStrategy : IParsingStrategy
    {
        public List<Article> Parse(string html, string domain)
        {
            var articles = new List<Article>();

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            foreach (var item in htmlDoc.DocumentNode.SelectNodes("//*[@data-id]"))
            {
                var article = new Article()
                {
                    Producer = Producers.Lanarte
                };

                article.Code = item.GetAttributeValue("data-id", "");

                var tagA = item.Descendants("a").FirstOrDefault(m => string.IsNullOrEmpty(m.GetAttributeValue("hyp-thumbnail", "")));

                if (tagA == null)
                {
                    // TODO: add some warning if html tag is null
                    continue;
                }

                article.Url = domain + tagA.GetAttributeValue("href", "");
                var image = tagA.Descendants("img").FirstOrDefault(m => string.IsNullOrEmpty(m.GetAttributeValue("data-src-original", "")));

                if (image == null)
                {
                    // TODO: add some warning if html tag is null
                    continue;
                }

                article.Title = image.GetAttributeValue("title", "");
                article.Img = $"{domain}/product/image/large/{article.Code}_1.jpg";

                articles.Add(article);
            }

            return articles;
        }
    }
}
