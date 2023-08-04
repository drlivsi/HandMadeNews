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
    public class ScrapFunc
    {
        private readonly ILogger<ScrapFunc> _logger;
        private readonly IScrapperService _scrapperService;

        public ScrapFunc(ILogger<ScrapFunc> logger, IScrapperService scrapperService)
        {
            _logger = logger;
            _scrapperService = scrapperService;
        }

        [FunctionName("Scrap")]
        public async Task<IActionResult> Scrap(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            await _scrapperService.DoScrap();
            await _scrapperService.SendToTelegram();


            string responseMessage = "This HTTP triggered function executed successfully";                

            return new OkObjectResult(responseMessage);
        }

        [FunctionName("ScrapByTimer")]
        public async Task ScrapByTimer([TimerTrigger("0 0 9 * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await _scrapperService.DoScrap();
            await _scrapperService.SendToTelegram();
        }
    }
}
