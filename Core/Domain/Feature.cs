using MongoDB.Bson.Serialization.Attributes;

namespace Core.Domain
{
    public class Feature
    {
        [BsonRequired]
        public string Key { get; set; }
        [BsonRequired]
        public string Value { get; set; }
    }
}