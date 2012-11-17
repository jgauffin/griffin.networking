using System.IO;
using System.Net;
using System.Text;
using Griffin.Networking.Buffers;
using Griffin.Networking.Http.Protocol;
using Griffin.Networking.Messaging;

namespace Griffin.Networking.Http.DemoServer.Basic
{
    /// <summary>
    /// Will handle all incoming HTTP requests.
    /// </summary>
    public class MyHttpService : HttpService
    {
        private static readonly BufferSliceStack Stack = new BufferSliceStack(50, 32000);

        /// <summary>
        /// Initializes a new instance of the <see cref="MyHttpService" /> class.
        /// </summary>
        public MyHttpService()
            : base(Stack)
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
        }

        /// <summary>
        /// A new message have been received from the remote end.
        /// </summary>
        /// <param name="message"></param>
        /// <remarks>We'll deserialize messages for you. What you receive here depends on the used <see cref="IMessageFormatterFactory"/>.</remarks>
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
}