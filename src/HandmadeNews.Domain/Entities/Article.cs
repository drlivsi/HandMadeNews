using HandmadeNews.Domain.Enums;
using HandmadeNews.Domain.SeedWork;

namespace HandmadeNews.Domain.Entities
{
    public class Article : Entity
    {
        public DateTime? CreatedAt { get; set; }
        public string Code { get; set; }
        public Producers Producer { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Img { get; set; }
        public ArticleStatus Status { get; set; }
        public bool TelegramRu { get; set; }
        public bool TelegramUa { get; set; }
    }
}
