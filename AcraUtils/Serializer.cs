using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace AcraUtils
{
    public class XMLSerializer
    {
        public static string Serialize<T>(T obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            try
            {
                XmlSerializer xmlserializer = new XmlSerializer(typeof(T));
                StringWriter stringWriter = new StringWriter();
                using (XmlWriter writer = XmlWriter.Create(stringWriter))
                {
                    xmlserializer.Serialize(writer, obj);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new SerializeException("Could not serialize", ex);
            }
        }

        public static T Unserialize<T>(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return default(T);
            }

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (TextReader reader = new StringReader(value))
                {
                    return (T)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                throw new UnserializeException("Could not unserialize", ex);
            }
        }
    }
    [Serializable()]
    public class SerializeException : Exception
    {
        public SerializeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
    [Serializable()]
    public class UnserializeException : Exception
    {
        public UnserializeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}