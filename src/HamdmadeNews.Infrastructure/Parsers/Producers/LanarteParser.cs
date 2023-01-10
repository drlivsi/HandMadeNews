using HandmadeNews.Domain;
using HtmlAgilityPack;

namespace HamdmadeNews.Infrastructure.Parsers.Producers
{
    public class LanarteParser : BaseParser
    {
        public override async Task<List<Article>> Parse(string pageUrl)
        {
            var articles = new List<Article>();

            var html = await CallUrl(pageUrl);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            foreach (var item in htmlDoc.DocumentNode.SelectNodes("//*[@data-id]"))
            {
                var article = new Article()
                {
                    Producer = HandmadeNews.Domain.Producers.Lanarte
                };

                article.Code = item.GetAttributeValue("data-id", "");

                var a = item.Descendants("a").FirstOrDefault(m => string.IsNullOrEmpty(m.GetAttributeValue("hyp-thumbnail", "")));

                article.Url = "https://webshop.verachtert.be" + a.GetAttributeValue("href", "");
                var image = a.Descendants("img").FirstOrDefault(m => string.IsNullOrEmpty(m.GetAttributeValue("data-src-original", "")));
                article.Title = image.GetAttributeValue("title", "");
                article.Img = "https://webshop.verachtert.be/product/image/large/" + article.Code + "_1.jpg";

                articles.Add(article);
            }

            return articles;
        }
    }
}
