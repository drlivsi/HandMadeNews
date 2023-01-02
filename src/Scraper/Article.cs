using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraper
{
    [Table("Article")]
    public class Article
    {
        [ExplicitKey]
        public string Id { get; set; }
        public string Producer { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Img { get; set; }       
    }
}
