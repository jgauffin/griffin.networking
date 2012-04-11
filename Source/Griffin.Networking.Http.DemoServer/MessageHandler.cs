using System;
using System.IO;
using Griffin.Networking.Http.Implementation;
using Griffin.Networking.Http.Messages;

namespace Griffin.Networking.Http.DemoServer
{
    public class MessageHandler : IUpstreamHandler
    {
        /// <summary>
        /// Handle an message
        /// </summary>
        /// <param name="context">Context unique for this handler instance</param>
        /// <param name="message">Message to process</param>
        /// <remarks>
        /// All messages that can't be handled MUST be send up the chain using <see cref="IPipelineHandlerContext.SendUpstream"/>.
        /// </remarks>
        public void HandleUpstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            var msg = message as ReceivedHttpRequest;
            if (msg == null)
                return;

            var request = msg.HttpRequest;
            var response = new HttpResponse(request.ProtocolVersion, 200, "OK");

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.WriteLine("Welcome dude!");
            writer.WriteLine("the time is: " + DateTime.Now);
            writer.Flush();

            stream.Position = 0;
            response.Body = stream;
            context.SendDownstream(new SendHttpResponse(response));
        }
    }
}