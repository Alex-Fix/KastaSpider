using System.Xml.Serialization;

namespace Core.Models.Kasta
{
    [XmlRoot(ElementName = "sitemap")]
    public class SitemapElement
    {
        [XmlElement(ElementName = "loc")]
        public string Loc { get; set; }
    }
}