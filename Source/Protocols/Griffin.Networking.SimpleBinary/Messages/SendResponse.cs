using System;
using Griffin.Networking.SimpleBinary.Services;

namespace Griffin.Networking.SimpleBinary.Messages
{
    /// <summary>
    /// Send a response
    /// </summary>
    /// <remarks>The object type must have been registered in the <see cref="ContentMapper"/>.</remarks>
    public class SendResponse : IPipelineMessage
    {
        /// <summary>
        /// Gets response to send.
        /// </summary>
        public object Response { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendResponse"/> class.
        /// </summary>
        /// <param name="response">The response.</param>
        public SendResponse(object response)
        {
            if (response == null) throw new ArgumentNullException("response");
            Response = response;
        }
    }
}
