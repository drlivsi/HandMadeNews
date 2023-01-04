namespace HandmadeNews.Domain
{
    public class Article
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public Producers Producer { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Img { get; set; }
        public ArticleStatus Status { get; set; }
        public bool TelegramRu { get; set; }
        public bool TelegramUa { get; set; }
    }

    public enum ArticleStatus
    {
        New,
        Approved,
        Rejected
    }

    public enum Producers
    {        
        Unknown,
        Lanarte,
        Bucilla,
    }
}
