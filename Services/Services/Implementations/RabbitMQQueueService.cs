using Core.Configurations;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Services.Implementations
{
    public class RabbitMQQueueService : IQueueService
    {
        #region Fields
        private readonly RabbitMQQueueConfiguration _rabbitMQQueueConfiguration;
        #endregion

        #region Constructor
        public RabbitMQQueueService(IOptions<RabbitMQQueueConfiguration> rabbitMQQueueConfiguration)
        {
            _rabbitMQQueueConfiguration = rabbitMQQueueConfiguration.Value;
        }
        #endregion

        #region Methods
        public void Insert<T>(string queueName, T entity)
        {
            if(string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentNullException(nameof(queueName));
            }

            var factory = new ConnectionFactory
            {
                HostName = _rabbitMQQueueConfiguration.HostName,
                UserName = _rabbitMQQueueConfiguration.UserName,
                Password = _rabbitMQQueueConfiguration.Password,
                Port = _rabbitMQQueueConfiguration.Port
            };
            using IConnection connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();

            channel.QueueDeclare(queue: queueName,
                                durable: false,
                                exclusive: false,
                                autoDelete: false, 
                                arguments: null);

            string serializedEntity = JsonConvert.SerializeObject(entity);
            byte[] body = Encoding.UTF8.GetBytes(serializedEntity);

            channel.BasicPublish(exchange: string.Empty, routingKey: queueName, basicProperties: null, body: body);
        }

        public void InsertRange<T>(string queueName, IEnumerable<T> entities, int batchSize = 1)
        {
            if (string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentNullException(nameof(queueName));
            }
            if(entities is null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var factory = new ConnectionFactory
            {
                HostName = _rabbitMQQueueConfiguration.HostName,
                UserName = _rabbitMQQueueConfiguration.UserName,
                Password = _rabbitMQQueueConfiguration.Password,
                Port = _rabbitMQQueueConfiguration.Port
            };
            using IConnection connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();

            channel.QueueDeclare(queue: queueName,
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

            foreach(var entity in entities.Chunk(batchSize))
            {
                string serializedEntity = JsonConvert.SerializeObject(entity);
                byte[] body = Encoding.UTF8.GetBytes(serializedEntity);
                channel.BasicPublish(exchange: string.Empty, routingKey: queueName, basicProperties: null, body: body);
            }
        }
        #endregion
    }
}