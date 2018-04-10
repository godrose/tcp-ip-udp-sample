using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using FluentAssertions;
using Newtonsoft.Json;
using ProtoBuf;
using UdpCommunication.Client;
using UdpCommunication.Server;
using Xunit;

namespace UdpCommunication.Tests
{
    public class InitTests : IDisposable
    {
        [Fact]
        public void SendMessage_SingleMessageIsSentSuccessfully_ReceivedMessageIsCorrect()
        {
            var port = 15000;
            var messageListener = new MessageListener(port);
            messageListener.Listen();            
                                        
            var server = new StreamingServer(port);
            server.SendMessage("Hello");
            Thread.Sleep(2000);

            var receivedMessages = messageListener.ReceivedMessages;
            receivedMessages.Should().BeEquivalentTo("Hello");
        }

        [Fact]
        public void SendMessage_MultipleMessagesAreSentSuccessfully_ReceivedMessagesAreCorrect()
        {
            var port = 15001;
            var messageListener = new MessageListener(port);
            messageListener.Listen();

            var server = new StreamingServer(port);
            server.SendMessage("Hello");
            server.SendMessage("World");
            Thread.Sleep(2000);

            var receivedMessages = messageListener.ReceivedMessages;
            receivedMessages.Should().BeEquivalentTo("Hello", "World");
        }

        [Theory]
        [InlineData(15100, false)]
        [InlineData(15101, true)]
        public void SendObject_SingleObjectIsSentSuccessfully_ReceivedObjectIsCorrect(int port, bool useProtobuf)
        {            
            var messageListener = new DataListener<SimpleObject>(port, useProtobuf);
            messageListener.Listen();

            var server = new StreamingServer(port, useProtobuf);
            server.SendObject("John");
            Thread.Sleep(2000);

            var receivedMessages = messageListener.ReceivedObjects.Cast<SimpleObject>();
            receivedMessages.Should().BeEquivalentTo(new SimpleObject
            {
                Name = "John"
            });
        }

        [Theory]
        [InlineData(15102,false)]
        [InlineData(15103, true)]
        public void SendObject_MultipleObjectsAreSentSuccessfully_ReceivedObjectsAreCorrect(int port, bool useProtobuf)
        {            
            var messageListener = new DataListener<SimpleObject>(port, useProtobuf);
            messageListener.Listen();

            var server = new StreamingServer(port, useProtobuf);
            server.SendObject("John");
            server.SendObject("Doe");
            Thread.Sleep(2000);

            var receivedMessages = messageListener.ReceivedObjects.Cast<SimpleObject>();
            receivedMessages.Should().BeEquivalentTo(new SimpleObject
            {
                Name = "John"
            }, new SimpleObject
            {
                Name = "Doe"
            });
        }        

        [JsonObject(MemberSerialization.OptOut)]
        [ProtoContract]
        class SimpleObject
        {
            [ProtoMember(1)]
            public string Name { get; set; }

            public override bool Equals(object obj)
            {
                var o = obj as SimpleObject;
                return o != null && o.Name == Name;
            }
        }

        public void Dispose()
        {
            var processes = Process.GetProcessesByName("UdpCommunication");
            foreach (var process in processes)
            {
                process.Kill();
            }
        }
    }
}
