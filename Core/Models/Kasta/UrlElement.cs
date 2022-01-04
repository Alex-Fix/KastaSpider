using System.Xml.Serialization;

namespace Core.Models.Kasta
{
    [XmlRoot(ElementName = "url")]
    public class UrlElement
    {
        [XmlElement(ElementName = "loc")]
        public string Loc { get; set; }
    }
}