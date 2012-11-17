using System;
using System.Net;
using Griffin.Networking.Messaging;
using Griffin.Networking.Protocols.Basic;

namespace BasicDemo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // factory that produces our service classes which
            // will handle all incoming messages
            var serviceFactory = new MyServiceFactory();

            // factory used to create the classes that will
            // serialize and build our messages
            var messageFactory = new BasicMessageFactory();

            // server configuration.
            // you can limit the number of clients etc.
            var configuration = new MessagingServerConfiguration(messageFactory);

            // actual server
            var server = new MessagingServer(serviceFactory, configuration);
            server.Start(new IPEndPoint(IPAddress.Any, 7652));

            var client = new MessagingClient(messageFactory);
            client.Connect(new IPEndPoint(IPAddress.Loopback, 7652));

            // Look here! We receive objects!
            client.Received += (sender, eventArgs) => Console.WriteLine("We received: " + eventArgs.Message);

            // And here we are sending one.
            client.Send(new OpenDoor {Id = Guid.NewGuid().ToString()});

            //to prevent the server from shutting down
            Console.ReadLine();
        }
    }
}