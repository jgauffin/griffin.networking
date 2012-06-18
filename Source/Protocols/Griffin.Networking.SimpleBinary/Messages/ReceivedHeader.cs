using System;

namespace Griffin.Networking.SimpleBinary.Messages
{
    /// <summary>
    /// We have recieved the header.
    /// </summary>
    public class ReceivedHeader : IPipelineMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivedHeader"/> class.
        /// </summary>
        /// <param name="header">The header.</param>
        public ReceivedHeader(SimpleHeader header)
        {
            if (header == null) throw new ArgumentNullException("header");
            Header = header;
        }

        /// <summary>
        /// Gets the recieved header
        /// </summary>
        public SimpleHeader Header { get; private set; }
    }
}