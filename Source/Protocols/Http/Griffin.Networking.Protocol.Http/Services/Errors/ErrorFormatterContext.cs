using System;
using Griffin.Networking.Http.Protocol;

namespace Griffin.Networking.Http.Services.Errors
{
    public class ErrorFormatterContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorFormatterContext"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        public ErrorFormatterContext(Exception exception, IRequest request, IResponse response)
        {
            Exception = exception;
            Request = request;
            Response = response;
        }

        /// <summary>
        /// Gets thrown exception
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Gets the response which will be sent back
        /// </summary>
        /// <remarks>The status code corresponds to the thrown exception. Fill the body with your error message.</remarks>
        public IResponse Response { get; private set; }

        /// <summary>
        /// Gets request which was handled when the error was generated.
        /// </summary>
        public IRequest Request { get; private set; }
    }
}