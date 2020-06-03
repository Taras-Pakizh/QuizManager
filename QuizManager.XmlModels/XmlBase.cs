using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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
                using (TextWriter streamWriter = new StreamWriter(memoryStream, Encoding.Unicode))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));

                    xmlSerializer.Serialize(streamWriter, xmlBase);

                    memoryStream.Position = 0;

                    using(XmlReader reader = XmlReader.Create(memoryStream))
                    {
                        return XElement.Load(reader);
                    }
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

        public static XElement SerializeAbstract(XmlBase obj)
        {
            //var type = property.PropertyType.GetGenericArguments()[0];

            //var methodInfo = typeof(DiagramWorker).GetRuntimeMethods()
            //    .Where(x => x.Name == "CompareList").Single();

            //var genericMethod = methodInfo.MakeGenericMethod(type);

            //genericMethod.Invoke(this, new object[]
            //    { property.GetValue(_correct), property.GetValue(_received), result });

            var methodInfo = typeof(XmlBase).GetRuntimeMethods().
                Where(x => x.Name == "_instanceSerializer").Single();

            var genericMethod = methodInfo.MakeGenericMethod(obj.GetType());

            return  (XElement)genericMethod.Invoke(obj, new object[] { obj });
        }

        protected XElement _instanceSerializer<T>(T xmlBase)
            where T : XmlBase
        {
            using (var memoryStream = new MemoryStream())
            {
                using (TextWriter streamWriter = new StreamWriter(memoryStream, Encoding.Unicode))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));

                    xmlSerializer.Serialize(streamWriter, xmlBase);

                    memoryStream.Position = 0;

                    using (XmlReader reader = XmlReader.Create(memoryStream))
                    {
                        return XElement.Load(reader);
                    }
                }
            }
        }
    }
}
