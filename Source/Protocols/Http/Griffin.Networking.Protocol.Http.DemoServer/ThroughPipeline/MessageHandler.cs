using System;
using System.IO;
using System.Net;
using System.Threading;
using Griffin.Networking.Http.Messages;
using Griffin.Networking.Pipelines;

namespace Griffin.Networking.Http.DemoServer.ThroughPipeline
{
    public class MessageHandler : IUpstreamHandler
    {
        #region IUpstreamHandler Members

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

            if (!Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                var authResponse = msg.HttpRequest.CreateResponse(HttpStatusCode.Unauthorized, "Authorize");
                context.SendDownstream(new SendHttpResponse(msg.HttpRequest, authResponse));
                return;
            }

            var request = msg.HttpRequest;
            var response = request.CreateResponse(HttpStatusCode.OK, "OK");

            if (request.Uri.AbsolutePath.Contains("submit.php") && request.Method == "POST")
            {
                Console.WriteLine(request.Form["arne"]);
            }
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.WriteLine("Welcome " + Thread.CurrentPrincipal.Identity.Name);
            writer.WriteLine("the time is: " + DateTime.Now);

            writer.Flush();

            stream.Position = 0;
            response.Body = stream;
            context.SendDownstream(new SendHttpResponse(request, response));
        }

        #endregion
    }
}