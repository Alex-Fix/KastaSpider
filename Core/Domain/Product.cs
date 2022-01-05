using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Core.Domain
{
    public class Product
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonRequired]
        public string Url { get; set; }
        [BsonRequired]
        public string Name { get; set; }
        [BsonIgnoreIfNull]
        public string Currency { get; set; }
        [BsonIgnoreIfDefault]
        public double Price { get; set; }
        [BsonIgnoreIfDefault]
        public double OldPrice { get; set; }
        [BsonIgnoreIfDefault]
        public double Discount { get; set; }
        [BsonIgnoreIfNull]
        public IEnumerable<string> ImageUrls { get; set; }
        [BsonIgnoreIfNull]
        public IEnumerable<string> Sizes { get; set; }
        [BsonIgnoreIfNull]
        public string Description { get; set; }
        [BsonIgnoreIfNull]
        public IEnumerable<Feature> Features { get; set; }
        [BsonIgnoreIfNull]
        public IEnumerable<string> Categories { get; set; }
    }
}