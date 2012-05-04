using System;
using Griffin.Networking.Channel;

namespace Griffin.Networking.Messages
{
    /// <summary>
    /// One of the handlers failed.
    /// </summary>
    /// <remarks>Will be sent when one of the handlers threw an exception that wasn't handled. The channel and pipeline will be cleaned up
    /// and <see cref="Closed"/> after this message have been handled.</remarks>
    public class PipelineFailure : IPipelineMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineFailure"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public PipelineFailure(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            Exception = exception;
        }

        /// <summary>
        /// Gets exception that was thrown by a handler in the pipeline
        /// </summary>
        public Exception Exception { get; private set; }
    }
}