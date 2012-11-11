using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Griffin.Networking.Http.Implementation;
using Griffin.Networking.Http.Messages;
using Griffin.Networking.Pipelines;
using Griffin.Networking.Pipelines.Messages;

namespace Griffin.Networking.Http.Handlers
{
    /// <summary>
    /// Used to catch all <see cref="PipelineFailure"/> and unhandled <see cref="ReceivedHttpRequest"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// 
    /// </para>
    /// Should be the last handler to be able to detect unhandled HTTP requests and to generate errors for all
    /// unprocessed <see cref="PipelineFailure"/>
    /// </remarks>
    public class PipelineFailureHandler : IUpstreamHandler
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
