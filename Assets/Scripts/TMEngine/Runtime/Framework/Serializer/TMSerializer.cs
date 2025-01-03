

namespace Tenmove.Runtime
{
    using System.IO;

    public interface ITMSerializer<T>
    {
        void Serialize(T serializeObject, Stream stream);
    }

    public interface ITMDeserializer<T>
    {
        void Deserialize(Stream stream,ref T serializeObject);
    }

    public class Serializer<TSerializer,T> where TSerializer : ITMSerializer<T>,new()
    {
        static public void Serialize(T serializeObject, Stream stream)
        {
            TSerializer serializer = new TSerializer();
            serializer.Serialize(serializeObject, stream);
        }
    }

    public class Deserializer<TDeserializer, T> where TDeserializer : ITMDeserializer<T>, new()
    {
        static public void Deserialize(Stream stream,ref T serializeObject)
        {
            TDeserializer deserializer = new TDeserializer();
            deserializer.Deserialize(stream,ref serializeObject);
        }
    }
}