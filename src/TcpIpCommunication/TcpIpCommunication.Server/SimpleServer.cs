using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using TcpIpCommunication.Data;

namespace TcpIpCommunication.Server
{
    public class SimpleServer
    {
        private readonly int _port;
        private TcpListener _listener;
        private Serializer _serializer;

        public SimpleServer(int port)
        {
            _port = port;
            ReceivedCommands = new List<string>();
            _serializer = new Serializer();
        }

        public void Listen()
        {
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), _port);
            _listener.Start();            
        }

        public async Task AcceptCommand()
        {
            var client = await _listener.AcceptTcpClientAsync();

            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[client.ReceiveBufferSize];

            int bytesRead = stream.Read(buffer, 0, client.ReceiveBufferSize);
            ReceivedCommands.Add(_serializer.Deserialize<Command>(buffer).Argument);
            stream.Write(buffer, 0, bytesRead);
            client.Close();
            _listener.Stop();
        }

        public List<string> ReceivedCommands { get; }
    }
}
