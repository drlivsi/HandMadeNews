namespace HandmadeNews.Infrastructure.Options
{
    public class TelegramOptions
    {
        public bool Enabled { get; set; }
        public string ApiKey { get; set; }
        public string ChatIdRu {  get; set; }
        public string ChatIdUa { get; set; }     
    }
}
