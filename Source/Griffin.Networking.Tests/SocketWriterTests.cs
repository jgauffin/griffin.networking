using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Griffin.Networking.Buffers;
using Xunit;

namespace Griffin.Networking.Tests
{
    public class SocketWriterTests
    {
        [Fact]
        public void SendOneMessage()
        {
            var connection = SocketTestTools.CreateConnection();
            var writer = new SocketWriter();
            writer.Assign(connection.Client);

            var msg = Encoding.ASCII.GetBytes("GET / HTTP/1.0\r\nServer: localhost\r\nContent-Length: 0\r\n\r\n");
            writer.Send(new SliceSocketWriterJob(new BufferSlice(msg, 0, msg.Length), msg.Length));

            var buffer = new byte[65535];
            Assert.Equal(msg.Length, connection.Server.Receive(buffer));
        }

        [Fact]
        public void DisconnectSend()
        {
            var connection = SocketTestTools.CreateConnection();
            var writer = new SocketWriter();
            writer.Assign(connection.Client);

            connection.Server.Shutdown(SocketShutdown.Send);
            connection.Server.Disconnect(false);

            var buffer2 = new byte[10];
            var bytes = connection.Client.Receive(buffer2);
            connection.Client.Shutdown(SocketShutdown.Send);
            connection.Client.Close();

            var msg = Encoding.ASCII.GetBytes("GET / HTTP/1.0\r\nServer: localhost\r\nContent-Length: 0\r\n\r\n");
            Assert.Throws<InvalidOperationException>(
                () => writer.Send(new SliceSocketWriterJob(new BufferSlice(msg, 0, msg.Length), msg.Length)));
        }

    }
}
