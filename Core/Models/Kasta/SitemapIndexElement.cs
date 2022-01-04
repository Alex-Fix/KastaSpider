using System.Collections.Generic;
using System.Xml.Serialization;

namespace Core.Models.Kasta
{
    [XmlRoot(ElementName = "sitemapindex", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
    public class SitemapIndexElement
    {
        [XmlElement(ElementName = "sitemap")]
        public List<SitemapElement> Sitemaps { get; set; }
    }
}