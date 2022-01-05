using AutoMapper;
using Core.Domain;
using Core.Models.Kasta;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Workers.Implementations
{
    public class ProductsParserWorker : IWorker<ProductsParserWorker, IEnumerable<string>>
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly IKastaClient _kastaClient;
        private readonly IProductService _productService;
        #endregion

        #region Constructor
        public ProductsParserWorker(
            IMapper mapper,
            IKastaClient kastaClient,
            IProductService productService)
        {
            _mapper = mapper;
            _kastaClient = kastaClient;
            _productService = productService;
        }
        #endregion

        #region Methods
        public async Task Execute(IEnumerable<string> productUrls)
        {
            if(!productUrls?.Any() ?? false)
            {
                throw new ArgumentException(nameof(productUrls));
            }

            var products = new List<Product>();
            foreach(var productUrl in productUrls)
            {
                ProductModel productModel = await _kastaClient.GetProduct(productUrl);
                if(productModel is null)
                {
                    continue;
                }

                products.Add(_mapper.Map<Product>(productModel));
            }

            if (products.Any())
            {
                await _productService.InsertRange(products);
            }
        }
        #endregion
    }
}