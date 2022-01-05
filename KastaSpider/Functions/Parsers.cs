using Core.Constants;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Services.Services;
using Services.Workers;
using Services.Workers.Implementations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KastaSpider.Functions
{
    public class Parsers
    {
        #region Fields
        private readonly ILogService _logService;
        private readonly IWorker<ProductsParserWorker, IEnumerable<string>> _productsParserWorker;
        #endregion

        #region Constructor
        public Parsers(
            ILogService logService,
            IWorker<ProductsParserWorker, IEnumerable<string>> productsParserWorker)
        {
            _logService = logService;
            _productsParserWorker = productsParserWorker;
        }
        #endregion

        #region Methods
        [FunctionName(nameof(ProductsParser))]
        public async Task ProductsParser([RabbitMQTrigger(QueueConstants.PRODUCT_URLS_QUEUE)]IEnumerable<string> productUrls, ILogger consoleLogger)
        {
            try
            {
                await _productsParserWorker.Execute(productUrls);
            }
            catch(Exception ex)
            {
                consoleLogger.LogError(ex.ToString());
                await _logService.Exception(ex);
                throw;
            }
        }
        #endregion
    }
}