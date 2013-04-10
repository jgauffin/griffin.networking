using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using FluentAssertions;
using Griffin.Networking.Buffers;
using Griffin.Networking.Servers;
using NSubstitute;
using Xunit;

namespace Griffin.Networking.Tests.Servers
{
    public class ServerClientContextTests
    {
        [Fact]
        public void RemoteDisconnectShouldCleanup()
        {
            var sockets = SocketTestTools.CreateConnection();
            var sut = new ServerClientContext(new BufferSlice(65535));
            var service = Substitute.For<INetworkService>();
            sut.Assign(sockets.Server, service);
            bool isDisconnected = false;
            sut.Disconnected += (sender, args) => isDisconnected = true;

            sockets.Client.Shutdown(SocketShutdown.Both);
            sockets.Client.Disconnect(false);
            Thread.Sleep(100);

            isDisconnected.Should()
                          .BeTrue("The receive callback should have triggered a disconnect and also cleaned up");
            sut.GetType().GetField("_socket", BindingFlags.Instance|BindingFlags.NonPublic).GetValue(sut).Should().BeNull("Since cleanup should have been made.");
        }

        [Fact]
        public void SocketErrorShouldCleanup()
        {
            var sockets = SocketTestTools.CreateConnection();
            var sut = new ServerClientContext(new BufferSlice(65535));
            var service = Substitute.For<INetworkService>();
            sut.Assign(sockets.Server, service);
            bool isDisconnected = false;
            sut.Disconnected += (sender, args) => isDisconnected = true;

            sockets.Client.Dispose();
            Thread.Sleep(100);

            isDisconnected.Should()
                          .BeTrue("The receive callback should have triggered a disconnect and also cleaned up");
            sut.GetType().GetField("_socket", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(sut).Should().BeNull("Since cleanup should have been made.");
        }

        [Fact]
        public void CloseShouldNotTriggerEvent()
        {
            var sockets = SocketTestTools.CreateConnection();
            var sut = new ServerClientContext(new BufferSlice(65535));
            var service = Substitute.For<INetworkService>();
            sut.Assign(sockets.Server, service);
            bool isDisconnected = false;
            sut.Disconnected += (sender, args) => isDisconnected = true;

            sut.Close();
            Thread.Sleep(100);

            isDisconnected.Should()
                          .BeFalse("The receive callback should have triggered a disconnect and also cleaned up");
            var socket = (Socket)sut.GetType().GetField("_socket", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(sut);
            socket.Should().BeNull("Should cleanup from the receive event not have been made.");
        }


        [Fact]
        public void ServerDisconnectThenSend()
        {
            var sockets = SocketTestTools.CreateConnection();
            var sut = new ServerClientContext(new BufferSlice(65535));
            var service = Substitute.For<INetworkService>();
            sut.Assign(sockets.Server, service);
            bool isDisconnected = false;
            sut.Disconnected += (sender, args) => isDisconnected = true;

            sockets.Server.Shutdown(SocketShutdown.Both);
            sut.Send(new BufferSlice(10), 10);
            Thread.Sleep(100);

            isDisconnected.Should()
                          .BeFalse("The receive callback should have triggered a disconnect and also cleaned up");
            var socket = (Socket)sut.GetType().GetField("_socket", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(sut);
            socket.Should().BeNull("Should cleanup from the receive event not have been made.");
        }


    }
}
