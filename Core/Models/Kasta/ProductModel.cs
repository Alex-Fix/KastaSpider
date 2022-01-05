using System.Collections.Generic;

namespace Core.Models.Kasta
{
    public class ProductModel
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public string Currency { get; set; }
        public double Price { get; set; }
        public double OldPrice { get; set; }
        public double Discount { get; set; }
        public IEnumerable<string> ImageUrls { get; set; }
        public IEnumerable<string> Sizes { get; set; }
        public string Description { get; set; }
        public IEnumerable<FeatureModel> Features { get; set; }
        public IEnumerable<string> Categories { get; set; }
    }
}