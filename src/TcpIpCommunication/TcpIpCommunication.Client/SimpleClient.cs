using System.Net.Sockets;
using TcpIpCommunication.Data;

namespace TcpIpCommunication.Client
{
    public class SimpleClient
    {
        private readonly int _port;
        private TcpClient _client;
        private Serializer _serializer;

        public SimpleClient(int port)
        {
            _port = port; 
            _serializer = new Serializer();           
        }

        public void Listen()
        {
            _client = new TcpClient("127.0.0.1", _port);                                              
        }

        public void SendCommand(Command command)
        {
            NetworkStream stream = _client.GetStream();
            byte[] bytesToSend = _serializer.Serialize(command);
            stream.Write(bytesToSend, 0, bytesToSend.Length);                        
            _client.Close();
        }        
    }
}
