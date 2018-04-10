using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using ProtoBuf;

namespace UdpCommunication.Client
{
    public class MessageListener
    {
        private readonly int _port;
        private readonly List<string> _receivedMessages = new List<string>();
        private readonly UdpClient _udp;

        public MessageListener(int port)
        {
            _port = port;
            _udp = new UdpClient(_port);
        }

        public void Listen()
        {
            _udp.BeginReceive(Receive, new object());
        }

        private void Receive(IAsyncResult ar)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, _port);
            byte[] bytes = _udp.EndReceive(ar, ref ip);
            _udp.BeginReceive(Receive, new object());
            _receivedMessages.Add(Encoding.UTF8.GetString(bytes));
        }

        public IEnumerable<string> ReceivedMessages => _receivedMessages;
    }

    public class DataListener<T> where T : class
    {
        private readonly int _port;
        private readonly bool _useProtobuf;
        private readonly List<T> _receivedObjects = new List<T>();
        private readonly UdpClient _udp;

        public DataListener(int port, bool useProtobuf = false)
        {
            _port = port;
            _useProtobuf = useProtobuf;
            _udp = new UdpClient(_port);
        }

        public void Listen()
        {
            _udp.BeginReceive(Receive, new object());
        }

        private void Receive(IAsyncResult ar)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, _port);
            byte[] bytes = _udp.EndReceive(ar, ref ip);
            _udp.BeginReceive(Receive, new object());
            _receivedObjects.Add(Deserialize(bytes));
        }

        private T Deserialize(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                if (_useProtobuf)
                {
                    return Serializer.Deserialize<T>(stream);
                }
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return JsonSerializer.Create().Deserialize(reader, typeof(T)) as T;
                }
            }
        }

        public IEnumerable<object> ReceivedObjects => _receivedObjects;
    }
}
