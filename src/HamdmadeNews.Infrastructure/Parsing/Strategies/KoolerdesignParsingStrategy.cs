using HandmadeNews.Domain.Entities;
using HandmadeNews.Domain.Enums;
using HtmlAgilityPack;
using System.Web;

namespace HamdmadeNews.Infrastructure.Parsing.Strategies
{
    public class KoolerdesignParsingStrategy : IParsingStrategy
    {
        public List<Article> Parse(string html, string domain)
        {
            var articles = new List<Article>();

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var table = htmlDoc.DocumentNode.SelectSingleNode("//table[@class='thumbnails4']");

            foreach (var item in table.Descendants("a").Take(10))
            {
                var article = new Article()
                {
                    Producer = Producers.KoolerDesign
                };

                article.Url = domain + item.GetAttributeValue("href", "");

                article.Code = HttpUtility.ParseQueryString(new Uri(article.Url).Query).Get("ProductID") ?? string.Empty;
                article.Img = domain + item.Descendants("img").FirstOrDefault()?.GetAttributeValue("src", "")?.Replace("_sm", "_lg");
                article.Title = item.Descendants("span").FirstOrDefault(m => m.GetAttributeValue("class", "") == "thumbname")?.InnerText ?? string.Empty;

                articles.Add(article);
            }

            return articles;
        }
    }
}