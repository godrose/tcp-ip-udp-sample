using FluentAssertions;
using TcpIpCommunication.Client;
using TcpIpCommunication.Data;
using TcpIpCommunication.Server;
using Xunit;

namespace TcpIpCommunication.Tests
{
    public class InteractionTests
    {
        [Fact]
        public async void SendMessage_SingleMessageIsSentSuccessfully_ReceivedMessageIsCorrect()
        {
            var port = 15000;
            var client = new SimpleClient(port);
            var server = new SimpleServer(port);

            server.Listen();
            client.Listen();
            client.SendCommand(new GetMessagesCommand("First message"));
            await server.AcceptCommand();           

            var receivedCommands = server.ReceivedCommands;
            receivedCommands.Should().BeEquivalentTo("First message");
        }        
    }

    class GetMessagesCommand : Command
    {
        public GetMessagesCommand(string argument) : base(argument)
        {
        }
    }
}
