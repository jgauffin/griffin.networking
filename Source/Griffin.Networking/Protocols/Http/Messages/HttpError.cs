using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Griffin.Networking.Messages;

namespace Griffin.Networking.Protocols.Http.Messages
{
    /// <summary>
    /// An error was caught during processing.
    /// </summary>
    public class HttpError : PipelineFailure
    {
        public HttpStatusCode StatusCode { get; private set; }

        public HttpError(HttpStatusCode statusCode, Exception exception) : base(exception)
        {
            StatusCode = statusCode;
        }
    }
}
