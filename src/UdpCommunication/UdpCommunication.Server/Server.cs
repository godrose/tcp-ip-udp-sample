using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using ProtoBuf;

namespace UdpCommunication.Server
{
    public class StreamingServer
    {
        private readonly int _port;
        private readonly bool _useProtobuf;

        public StreamingServer(int port, bool useProtobuf = false)
        {
            _port = port;
            _useProtobuf = useProtobuf;
        }

        public void SendMessage(string message)
        {
            UdpClient client = new UdpClient();
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("255.255.255.255"), _port);
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            client.Send(bytes, bytes.Length, ip);
            client.Close();
        }

        public void SendObject(string name)
        {
            UdpClient client = new UdpClient();
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("255.255.255.255"), _port);
            byte[] bytes = Serialize(new SimpleObject
            {
                Name = name
            });
            client.Send(bytes, bytes.Length, ip);
            client.Close();
        }

        private byte[] Serialize<T>(T @object)
        {
            if (_useProtobuf == false)
            {
                return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@object));
            }
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, @object);
                stream.Seek(0, SeekOrigin.Begin);
                return stream.ToArray();
            }
        }        
    }

    [JsonObject(MemberSerialization.OptOut)]
    [ProtoContract]
    class SimpleObject
    {
        [ProtoMember(1)]
        public string Name { get; set; }
    }
}
