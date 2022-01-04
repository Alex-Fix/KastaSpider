using Azure.Storage.Queues;
using Core.Configurations;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Services.Implementations
{
    public class AzureQueueService : IQueueService
    {
        private readonly AzureQueueConfiguration _azureQueueConfiguration;

        public AzureQueueService(IOptions<AzureQueueConfiguration> azureQueueConfiguration)
        {
            _azureQueueConfiguration = azureQueueConfiguration.Value;
        }

        public void Insert<T>(string queueName, T entity)
        {
            if (string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentNullException(queueName);
            }

            string serializedEntity = JsonConvert.SerializeObject(entity);
            string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(serializedEntity));

            var queueClient = new QueueClient(_azureQueueConfiguration.StorageConnectionString, queueName);
            queueClient.CreateIfNotExists();
            queueClient.SendMessage(base64);
        }

        public void InsertRange<T>(string queueName, IEnumerable<T> entities, int batchSize = 1)
        {
            if (string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentNullException(queueName);
            }
            if(entities is null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var queueClient = new QueueClient(_azureQueueConfiguration.StorageConnectionString, queueName);
            queueClient.CreateIfNotExists();

            foreach(var entity in entities.Chunk(batchSize))
            {
                string serializedEntity = JsonConvert.SerializeObject(entity);
                string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(serializedEntity));
                queueClient.SendMessage(base64);
            }
        }
    }
}