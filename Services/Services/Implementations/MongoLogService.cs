using Core.Domain;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace Services.Services.Implementations
{
    public class MongoLogService : ILogService
    {
        #region Constants
        private const string COLLECTION_NAME = "Logs";
        #endregion

        #region Fields
        private readonly IMongoDatabase _mongoDatabase;
        #endregion

        #region Constructor
        public MongoLogService(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }
        #endregion

        #region Methods
        public async Task Exception(Exception ex)
        {
            var log = new ExceptionLog
            {
                CreatedUtc = DateTime.UtcNow,
                Exception = ex.ToString()
            };

            IMongoCollection<ExceptionLog> collection = _mongoDatabase.GetCollection<ExceptionLog>(COLLECTION_NAME);

            await collection.InsertOneAsync(log);
        }
        #endregion
    }
}