using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Core.Domain
{
    public class ExceptionLog
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public string Exception { get; set; }
    }
}