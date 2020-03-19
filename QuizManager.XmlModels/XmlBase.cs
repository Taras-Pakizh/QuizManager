using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace QuizManager.XmlModels
{
    [Serializable]
    public abstract class XmlBase
    {
        public static XElement Serialize<T>(T xmlBase)
            where T : XmlBase
        {
            using (var memoryStream = new MemoryStream())
            {
                using (TextWriter streamWriter = new StreamWriter(memoryStream))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));

                    xmlSerializer.Serialize(streamWriter, xmlBase);

                    return XElement.Parse(Encoding.ASCII.GetString(memoryStream.ToArray()));
                }
            }
        }

        public static XmlBase Deserialize(XElement element, string typeName)
        {
            var type = Type.GetType("QuizManager.XmlModels." + typeName);

            var xmlSerializer = new XmlSerializer(type);

            return (XmlBase)xmlSerializer.Deserialize(element.CreateReader());
        }

        public static Type GetType(string typeName)
        {
            return Type.GetType("QuizManager.XmlModels." + typeName);
        }
    }
}
