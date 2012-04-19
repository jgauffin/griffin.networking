using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Griffin.Networking.Http.Implementation;
using Griffin.Networking.Http.Messages;
using Griffin.Networking.Messages;

namespace Griffin.Networking.Http.Handlers
{
    /// <summary>
    /// Sends HTTP errors back
    /// </summary>
    /// <remarks>
    /// Should be the last handle to be able to detect 404.
    /// </remarks>
    public class ErrorHandler : IUpstreamHandler
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
            var failure = message as PipelineFailure;
            if (failure != null)
            {
                var response = new HttpResponse("HTTP/1.1", HttpStatusCode.InternalServerError, "Server failed!");
                response.Body = new MemoryStream();
                var buffer = Encoding.ASCII.GetBytes(failure.Exception.ToString());
                response.Body.Write(buffer, 0, buffer.Length);
                response.Body.Position = 0;
                context.SendDownstream(new SendHttpResponse(null, response));
                return;
            }

            var requestMsg = message as ReceivedHttpRequest;
            if(requestMsg != null)
            {
                var response = new HttpResponse("HTTP/1.1", HttpStatusCode.NotFound, "Failed to find " + requestMsg.HttpRequest.Uri.AbsolutePath);
                context.SendDownstream(new SendHttpResponse(null, response));
            }
        }
    }
}
