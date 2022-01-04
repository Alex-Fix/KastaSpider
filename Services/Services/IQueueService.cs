using System.Collections.Generic;

namespace Services.Services
{
    public interface IQueueService
    {
        void Insert<T>(string queueName, T entity);
        void InsertRange<T>(string queueName, IEnumerable<T> entities, int batchSize = 1);
    }
}