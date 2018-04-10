namespace UdpCommunication.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new StreamingServer(15000);
            server.SendMessage("Test");
        }
    }
}
