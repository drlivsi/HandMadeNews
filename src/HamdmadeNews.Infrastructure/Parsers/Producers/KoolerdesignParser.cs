using HandmadeNews.Domain;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HamdmadeNews.Infrastructure.Parsers.Producers
{
    public class KoolerdesignParser : BaseParser
    {
        public override async Task<List<Article>> Parse(string pageUrl)
        {
            var articles = new List<Article>();

            var html = await CallUrl(pageUrl);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var table = htmlDoc.DocumentNode.SelectSingleNode("//table[@class='thumbnails4']");

            foreach (var item in table.Descendants("a").Take(10))
            {
                var article = new Article()
                {
                    Producer = HandmadeNews.Domain.Producers.KoolerDesign
                };

                article.Url = "https://www.koolerdesign.com/" + item.GetAttributeValue("href", "");
                
                article.Code = HttpUtility.ParseQueryString(new Uri(article.Url).Query).Get("ProductID") ?? string.Empty;
                article.Img = "https://www.koolerdesign.com/" + item.Descendants("img").FirstOrDefault()?.GetAttributeValue("src", "")?.Replace("_sm", "_lg") ?? string.Empty;

                article.Title = item.Descendants("span").FirstOrDefault(m => m.GetAttributeValue("class", "") == "thumbname")?.InnerText ?? string.Empty;

                articles.Add(article);
            }

            return articles;
        }
    }
}