using Core.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Services.Services;
using Services.Workers;
using Services.Workers.Implementations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KastaSpider
{
    public class Fetcher
    {
        private readonly ILogService _logService;
        private readonly IWorker<SitemapsFetcherWorker, object> _sitemapsFetcherWorker;
        private readonly IWorker<ProductsFetcherWorker, IEnumerable<string>> _productsFetcherWorker;

        public Fetcher(
            ILogService logService,
            IWorker<SitemapsFetcherWorker, object> sitemapsFetcherWorker,
            IWorker<ProductsFetcherWorker, IEnumerable<string>> productsFetcherWorker)
        {
            _logService = logService;
            _sitemapsFetcherWorker = sitemapsFetcherWorker;
            _productsFetcherWorker = productsFetcherWorker;
        }

        [FunctionName(nameof(SitemapsFetcher))]
        public async Task<IActionResult> SitemapsFetcher([HttpTrigger(AuthorizationLevel.Function, "GET")] HttpRequest request, ILogger consoleLogger)
        {
            try
            {
                await _sitemapsFetcherWorker.Execute();
                return new OkResult();
            }
              catch(Exception ex)
            {
                consoleLogger.LogError(ex.ToString());
                await _logService.Exception(ex);
                throw;
            }
        }

        [FunctionName(nameof(ProductsFetcher))]
        public async Task ProductsFetcher([RabbitMQTrigger(QueueConstants.PRODUCT_SITEMAPS_QUEUE)] IEnumerable<string> productSitemapUrls, ILogger consoleLogger)
        {
            try
            {
                await _productsFetcherWorker.Execute(productSitemapUrls);
            }
            catch (Exception ex)
            {
                consoleLogger.LogError(ex.ToString());
                await _logService.Exception(ex);
                throw;
            }
        }
    }
}