using Core.Configurations;
using Core.Constants;
using Microsoft.Extensions.Options;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Workers.Implementations
{
    public class ProductsFetcherWorker : IWorker<ProductsFetcherWorker, IEnumerable<string>>
    {
        private readonly IKastaClient _kastaClient;
        private readonly IQueueService _queueService;
        private readonly KastaSpiderConfiguration _kastaSpiderConfiguration;

        public ProductsFetcherWorker(
            IKastaClient kastaClient,
            IQueueService queueService,
            IOptions<KastaSpiderConfiguration> kastaSpiderConfiguration)
        {
            _kastaClient = kastaClient;
            _queueService = queueService;
            _kastaSpiderConfiguration = kastaSpiderConfiguration.Value;
        }

        public async Task Execute(IEnumerable<string> productSitemapUrls)
        {
            if(!productSitemapUrls?.Any() ?? false)
            {
                throw new ArgumentException($"{nameof(productSitemapUrls)} can`t be null or empty");
            }

            foreach(var productSitemapUrl in productSitemapUrls)
            {
                IList<string> productUrls = await _kastaClient.GetProductUrls(productSitemapUrl);
                if (!productUrls.Any())
                {
                    throw new InvalidOperationException($"{nameof(productUrls)} can`t be empty");
                }

                _queueService.InsertRange(
                    QueueConstants.PRODUCT_URLS_QUEUE,
                    productUrls,
                    _kastaSpiderConfiguration.ProductsFetcgerBatchSize);
            }
        }
    }
}