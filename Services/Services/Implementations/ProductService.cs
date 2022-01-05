using Core.Domain;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Services.Implementations
{
    public class ProductService : IProductService
    {
        #region Constants
        private const string COLLECTION_NAME = "Products";
        #endregion

        #region Fields
        private readonly IMongoDatabase _mongoDatabase;
        #endregion

        #region Constructor
        public ProductService(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }
        #endregion

        #region Methods
        public async Task InsertRange(IEnumerable<Product> products)
        {
            if(products is null)
            {
                throw new ArgumentNullException(nameof(products));
            }

            IMongoCollection<Product> collection = _mongoDatabase.GetCollection<Product>(COLLECTION_NAME);

            var productUrls = products.Select(p => p.Url).ToList();

            var existsProductsCursor = await collection.FindAsync(p => productUrls.Contains(p.Url));
            var existsProducts = await existsProductsCursor.ToListAsync();

            var productsToReplace = existsProducts
                .Select(ep => 
                {
                    var product = products.FirstOrDefault(p => p.Url == ep.Url);
                    product.Id = ep.Id;

                    return (WriteModel<Product>)new ReplaceOneModel<Product>(Builders<Product>.Filter.Eq(p => p.Id, ep.Id), product);
                })
                .ToList();
            var productsToInsert = productUrls
                .Except(existsProducts.Select(ep => ep.Url))
                .Select(pu => (WriteModel<Product>)new InsertOneModel<Product>(products.First(p => p.Url == pu)))
                .ToList();

            await collection.BulkWriteAsync(productsToReplace.Union(productsToInsert));
        }
        #endregion
    }
}