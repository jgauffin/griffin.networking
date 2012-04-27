using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Griffin.Networking.Buffers;
using Griffin.Networking.Channels;
using Griffin.Networking.Logging;
using Griffin.Networking.Messages;
using Xunit;

namespace Griffin.Networking.Tests.Channels
{
    public class TcpChannelTest
    {
        private MyPipeline _pipeline;
        private TestConnection _sockets;
        private TcpServerChildChannel _target;

        public TcpChannelTest()
        {
            LogManager.Assign(new SimpleLogManager<ConsoleLogger>());
            _pipeline = new MyPipeline();
            _sockets = SocketTestTools.CreateConnection();
            _target = new TcpServerChildChannel(_pipeline);
            _target.AssignSocket(_sockets.Client);
            _target.StartChannel();
        }


        [Fact]
        public void FillBuffers()
        {
            LogManager.Assign(new SimpleLogManager<ConsoleLogger>());

            var pool = new BufferPool(100, 2, 2);
            _target = new TcpServerChildChannel(_pipeline, pool);
            _target.AssignSocket(_sockets.Client);
            _target.StartChannel();

            var sb = new StringBuilder();
            for (int i = 0; i < 1000; i++)
            {
                sb.Append(i.ToString());
                sb.Append("|");
                if ((i % 25) == 0)
                    sb.AppendLine();
            }
            var sendBuffer = Encoding.ASCII.GetBytes(sb.ToString());
            var sent = 0;
            while (sent < sendBuffer.Length)
            {
                sent += _sockets.Server.Send(sendBuffer, sent, sendBuffer.Length - sent, SocketFlags.None);
            }

            Assert.True(_pipeline.WaitOnUpstream<Received>(TimeSpan.FromMilliseconds(1000)), "Incoming message timeout");

            var receivedBuffer = new byte[sendBuffer.Length];
            var stream = new MemoryStream(receivedBuffer);
            foreach (var msg in _pipeline.UpstreamMessages)
            {
                var m = msg as Received;
                if (m == null)
                    continue;

                stream.Write(m.BufferSlice.Buffer, m.BufferSlice.Position, m.BufferSlice.RemainingLength);
            }
            stream.Flush();

            Assert.Equal(sendBuffer.Length, receivedBuffer.Length);
            for (int i = 0; i < sendBuffer.Length; i++)
            {
                if (sendBuffer[i] != receivedBuffer[i])
                    throw new InvalidOperationException("First difference at " + i);
            }

        }

        [Fact]
        public void ReceiveMessage()
        {
            var buffer = Encoding.UTF8.GetBytes("Hello world");
            Assert.Equal(buffer.Length, _sockets.Server.Send(buffer));

            Assert.True(_pipeline.WaitOnUpstream<Received>(TimeSpan.FromSeconds(1)), "Failed to receive a message");
            var firstMessage = _pipeline.UpstreamMessages.First();
            var msg = (Received)firstMessage;


            Assert.Equal(buffer, msg.BufferSlice.Buffer.Take(msg.BufferSlice.Count).ToArray());
        }

        [Fact]
        public void ReceiveMessages()
        {
            var buffer = Encoding.UTF8.GetBytes("Hello world");
            Assert.Equal(buffer.Length, _sockets.Server.Send(buffer));

            Action<Received> callback = m =>
                                            {
                                                Assert.Equal(buffer,
                                                             m.BufferSlice.Buffer.Take(m.BufferSlice.Count).ToArray());
                                                m.BufferSlice.Position += m.BufferSlice.Count;
                                            };

            Assert.True(_pipeline.WaitOnUpstream(TimeSpan.FromSeconds(10000), callback));

            buffer = Encoding.UTF8.GetBytes("Hello world2!");
            Assert.Equal(buffer.Length, _sockets.Server.Send(buffer));
            Assert.True(_pipeline.WaitOnUpstream(TimeSpan.FromSeconds(10000), callback));
        }

        [Fact]
        public void SendMessage()
        {
            var buffer = Encoding.UTF8.GetBytes("Hello world");
            _target.Send(new SendMessage(new BufferSlice(buffer, 0, buffer.Length, buffer.Length)));

            var buffer2 = new byte[buffer.Length];
            _sockets.Server.Receive(buffer2, 0, buffer2.Length, SocketFlags.None);
            Assert.Equal(buffer, buffer2);
        }

        [Fact]
        public void TestDisconnect()
        {
            _sockets.Server.Disconnect(false);
            _sockets.Server.Close();
            var buffer = Encoding.UTF8.GetBytes("Hello world");
            _target.Send(new SendMessage(new BufferSlice(buffer, 0, buffer.Length, buffer.Length)));

            Assert.True(_pipeline.WaitOnUpstream<Disconnected>(TimeSpan.FromSeconds(1)), "Did not get disconnect message.");
            Assert.Throws<InvalidOperationException>(
                () => _target.Send(new SendMessage(new BufferSlice(new byte[1], 0, 1, 0))));
        }


    }
}
