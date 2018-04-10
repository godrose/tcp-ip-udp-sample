using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace TcpIpCommunication.Data
{
    public class Serializer
    {
        public byte[] Serialize<T>(T @object)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@object));
        }

        public T Deserialize<T>(byte[] data) where T : class
        {
            using (var stream = new MemoryStream(data))
            {                
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return JsonSerializer.Create().Deserialize(reader, typeof(T)) as T;
                }
            }
        }
    }
}