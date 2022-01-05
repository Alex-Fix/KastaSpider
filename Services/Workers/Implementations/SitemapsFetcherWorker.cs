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
    public class SitemapsFetcherWorker : IWorker<SitemapsFetcherWorker, object>
    {
        #region Fields
        private readonly IKastaClient _kastaClient;
        private readonly IQueueService _queueService;
        private readonly KastaSpiderConfiguration _kastaSpiderConfiguration;
        #endregion

        #region Constructor
        public SitemapsFetcherWorker(
            IKastaClient kastaClient,
            IQueueService queueService,
            IOptions<KastaSpiderConfiguration> kastaSpiderConfiguration)
        {
            _kastaClient = kastaClient;
            _queueService = queueService;
            _kastaSpiderConfiguration = kastaSpiderConfiguration.Value;
        }
        #endregion

        #region Methods
        public async Task Execute(object data = default)
        {
            IList<string> productSitemapUrls = await _kastaClient.GetProductSitemapUrls();
            if (!productSitemapUrls.Any())
            {
                throw new InvalidOperationException($"{nameof(productSitemapUrls)} is empty");
            }

            _queueService.InsertRange(
                QueueConstants.PRODUCT_SITEMAPS_QUEUE, 
                productSitemapUrls, 
                _kastaSpiderConfiguration.SitemapsFetcherBatchSize);
        }
        #endregion
    }
}