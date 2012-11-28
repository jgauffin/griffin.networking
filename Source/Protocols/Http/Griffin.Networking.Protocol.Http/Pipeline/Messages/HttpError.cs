using System;
using System.Net;
using Griffin.Networking.Pipelines.Messages;

namespace Griffin.Networking.Protocol.Http.Pipeline.Messages
{
    /// <summary>
    /// An error was caught during processing.
    /// </summary>
    public class HttpError : PipelineFailure
    {
        public HttpError(HttpStatusCode statusCode, Exception exception) : base(exception)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; private set; }
    }
}