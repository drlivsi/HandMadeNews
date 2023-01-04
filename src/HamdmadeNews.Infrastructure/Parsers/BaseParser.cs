using HamdmadeNews.Infrastructure.Scrapers;
using HandmadeNews.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HamdmadeNews.Infrastructure.Parsers
{
    public abstract class BaseParser : IParser
    {     
        public abstract Task<List<Article>> Parse(string pageUrl);        

        protected async Task<string> CallUrl(string fullUrl)
        {
            var client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            client.DefaultRequestHeaders.Accept.Clear();
            var response = client.GetStringAsync(fullUrl);
            return await response;
        }       
    }
}
