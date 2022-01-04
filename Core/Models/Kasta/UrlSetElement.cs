using System.Collections.Generic;
using System.Xml.Serialization;

namespace Core.Models.Kasta
{
    [XmlRoot(ElementName = "urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
    public class UrlSetElement
    {
        [XmlElement(ElementName = "url")]
        public List<UrlElement> Urls { get; set; }
    }
}