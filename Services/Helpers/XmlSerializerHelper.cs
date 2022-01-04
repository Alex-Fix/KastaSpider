using System.IO;
using System.Xml.Serialization;

namespace Services.Helpers
{
    public static class XmlSerializerHelper
    {
        public static T Deserialize<T>(string serializedObj) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using StringReader reader = new StringReader(serializedObj);

            return (T)serializer.Deserialize(reader);
        }

        public static string Serialize<T>(T obj) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            using StringWriter writer = new StringWriter();

            serializer.Serialize(writer, obj);
            return writer.ToString();
        }
    }
}