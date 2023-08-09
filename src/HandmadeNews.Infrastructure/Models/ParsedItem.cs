namespace HandmadeNews.Infrastructure.Models
{
    public class ParsedItem
    {
        public Type ProducerType { get; set; }
        public Uri Url { get; set; }
        public string Html { get; set; }

        public string Domain => Url.Scheme + "://" + Url.Host;
    }
}
