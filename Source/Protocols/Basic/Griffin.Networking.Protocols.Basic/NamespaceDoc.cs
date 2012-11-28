using System.Runtime.CompilerServices;

namespace Griffin.Networking.Protocol.Basic
{
    /// <summary>
    /// Lightweight transportation of objects over the wire.
    /// </summary>
    /// <remarks>Uses json.net to serialize/deserialize the objects. The JSON is wrapped by a binary header (byte = version, int = XML size).
    /// <para>Simply construct a client or server using the <see cref="BasicMessageFactory"/></para>
    /// </remarks>
    /// <example>
    /// Server:
    /// <code>
    /// // configuration
    /// var serviceFactory = new MyServiceFactory();
    /// var messageFactory = new BasicMessageFactory();
    /// var configuration = new MessagingServerConfiguration(messageFactory);
    /// 
    /// // start
    /// var server = new MessagingServer(serviceFactory, configuration);
    /// server.Start(new IPEndPoint(IPAddress.Any, 7652));
    /// 
    /// // class handling the requests
    /// public class MyService : MessagingService
    /// {
    /// 	public override void HandleReceive(object message)
    /// 	{
    /// 		// We can only receive this kind of command
    /// 		var msg = (OpenDoor) message;
    /// 
    /// 		Console.WriteLine("Should open door: {0}.", msg.Id);
    /// 
    /// 		// Send a reply
    /// 		Context.Write(new DoorOpened(msg.Id));
    /// 	}
    /// }
    /// </code>
    /// Client:
    /// <code>
    /// // configuration
    /// var messageFactory = new BasicMessageFactory();
    /// 
    /// var client = new MessagingClient(messageFactory);
    /// client.Connect(new IPEndPoint(IPAddress.Loopback, 7652));
    /// 
    /// // Look here! We receive objects!
    /// client.Received += (sender, eventArgs) => Console.WriteLine("We received: " + eventArgs.Message);
    /// 
    /// // And here we are sending one.
    /// client.Send(new OpenDoor {Id = Guid.NewGuid().ToString()});
    /// </code>
    /// </example>
    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }
}