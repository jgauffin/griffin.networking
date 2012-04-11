using System.Net;
using System.Net.Sockets;

namespace Griffin.Networking.Tests
{
    public static class SocketTestTools
    {
        public static TestConnection CreateConnection()
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var ar = listener.BeginAcceptSocket(null, null);
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            s.Connect(IPAddress.Loopback, ((IPEndPoint)listener.LocalEndpoint).Port);
            var server = listener.EndAcceptSocket(ar);
            return new TestConnection {Client = s, Server = server};
        }
    }

    public class TestConnection
    {
        public Socket Server { get; set; }
        public Socket Client { get; set; }
    }
}
