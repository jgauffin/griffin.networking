using System.Runtime.CompilerServices;

namespace Griffin.Networking.Clients
{
    /// <summary>
    /// This namespace contains classes which are used at the client side.
    /// </summary>
    /// <example>
    /// <code>
    /// class Program
    /// {
    /// 	static void Main(string[] args)
    /// 	{
    /// 		//Setup, we are using the built in formatter which can transport any POCO (using XML with a binary header)
    /// 		var client = new MessagingClient(new BasicMessageFormatterFactory());
    /// 		client.Received += OnMessage;
    /// 
    /// 		//Connect to server
    /// 		client.Connect(new IPEndPoint(IPAddress.Loopback, 8844));
    /// 
    /// 		// Send a message
    /// 		client.Send(new InvokeME(){Hello = "World"});
    /// 
    /// 		Console.ReadLine();
    /// 
    /// 	}
    /// 
    /// 	// Receive a message from the server
    /// 	private static void OnMessage(object sender, ReceivedMessageEventArgs e)
    /// 	{
    /// 		var msg = (Pong) e.Message;
    /// 		Console.WriteLine("Got ponged from server. The pong was sent at " + msg.PongedAt);
    /// 	}
    /// }
    /// </code>
    /// </example>
    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }
}