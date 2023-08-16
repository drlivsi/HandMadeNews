using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using HandmadeNews.Infrastructure.Services;
using HandmadeNews.Infrastructure.Options;
using HandmadeNews.Infrastructure.Parsing.Strategies;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace HandmadeNews.AzureFunc
{
    public class ParserFunc
    {
        private readonly ILogger<ParserFunc> _logger;
        private readonly IParsingService _parsingService;
        private readonly IOptions<ProducersOptions> _producersOptions;

        public ParserFunc(ILogger<ParserFunc> logger, IParsingService scrapperService, IOptions<ProducersOptions> producersOptions)
        {
            _logger = logger;
            _parsingService = scrapperService;
            _producersOptions = producersOptions;
        }

        [FunctionName("Parse")]
        public async Task<IActionResult> Parse(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            await Parse();

            string responseMessage = "This HTTP triggered function executed successfully";                

            return new OkObjectResult(responseMessage);
        }

        [FunctionName("ParseByTimer")]
        public async Task ParseByTimer([TimerTrigger("0 0 9 * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await Parse();
        }

        private Dictionary<Type, string> GetProducers()
        {
            var producers = new Dictionary<Type, string>();
            producers.Add(typeof(LanarteParsingStrategy), _producersOptions.Value.LanarteUrl);
            producers.Add(typeof(BucillaParsingStrategy), _producersOptions.Value.BucillaUrl);
            producers.Add(typeof(KoolerdesignParsingStrategy), _producersOptions.Value.KoolerDesignUrl);
            return producers;
        }

        private async Task Parse()
        {
            var producers = GetProducers();
            var parsedItems = await _parsingService.DownloadAsync(producers);
            var articles = _parsingService.Parse(parsedItems);
            await _parsingService.SaveIfNewAsync(articles);
            await _parsingService.SendToTelegram();
        }
    }
}
