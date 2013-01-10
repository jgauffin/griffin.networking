Griffin.Networking
==================

High performance networking library for .NET.

A public beta has been released (nuget package).

## Documentation

* [Blog introduction](http://blog.gauffin.org/2012/05/griffin-networking-a-somewhat-performant-networking-library-for-net/)
* [Messaging introduction](http://blog.gauffin.org/2012/11/new-version-of-griffin-networking-a-networking-library-for-net/)
* [MSDN style docs](http://griffinframework.net/docs/networking)
* [Forum/Mailing list](https://groups.google.com/forum/?fromgroups#!forum/griffin-networking)

Still work in progress but the core framework should be reasonable stable.

## Example HTTP listener

	internal class Program
	{
		public static void RunDemo()
		{
			var server = new MessagingServer(new MyHttpServiceFactory(),
												new MessagingServerConfiguration(new HttpMessageFactory()));
			server.Start(new IPEndPoint(IPAddress.Loopback, 8888));
		}
	}
	 
	// factory
	public class MyHttpServiceFactory : IServiceFactory
	{
		public IServerService CreateClient(EndPoint remoteEndPoint)
		{
			return new MyHttpService();
		}
	}
	 
	// and the handler
	public class MyHttpService : HttpService
	{
		private static readonly BufferSliceStack Stack = new BufferSliceStack(50, 32000);
	 
		public MyHttpService()
			: base(Stack)
		{
		}
	 
		public override void Dispose()
		{
		}
	 
		public override void HandleReceive(object message)
		{
			var msg = (IRequest) message;
	 
			var response = msg.CreateResponse(HttpStatusCode.OK, "Welcome");
	 
			response.Body = new MemoryStream();
			response.ContentType = "text/plain";
			var buffer = Encoding.UTF8.GetBytes("Hello world");
			response.Body.Write(buffer, 0, buffer.Length);
			response.Body.Position = 0;
	 
			Send(response);
		}
	}
