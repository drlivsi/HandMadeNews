using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using HamdmadeNews.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Net.WebRequestMethods;

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

        [FunctionName("ScrapManually")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            await _scrapperService.DoScrap();
            await _scrapperService.SendToTelegram();


            string responseMessage = "This HTTP triggered function executed successfully";                

            return new OkObjectResult(responseMessage);
        }

        [FunctionName("ScrapTimer")]
        public async Task Run2([TimerTrigger("0 0 9 * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await _scrapperService.DoScrap();
            await _scrapperService.SendToTelegram();
        }
    }
}
