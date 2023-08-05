using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using HandmadeNews.Infrastructure.Services;

namespace HandmadeNews.AzureFunc
{
    public class ParserFunc
    {
        private readonly ILogger<ParserFunc> _logger;
        private readonly IParsingService _parsingService;

        public ParserFunc(ILogger<ParserFunc> logger, IParsingService scrapperService)
        {
            _logger = logger;
            _parsingService = scrapperService;
        }

        [FunctionName("Parse")]
        public async Task<IActionResult> Parse(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            await _parsingService.Parse();
            await _parsingService.SendToTelegram();


            string responseMessage = "This HTTP triggered function executed successfully";                

            return new OkObjectResult(responseMessage);
        }

        [FunctionName("ParseByTimer")]
        public async Task ParseByTimer([TimerTrigger("0 0 9 * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await _parsingService.Parse();
            await _parsingService.SendToTelegram();
        }
    }
}
