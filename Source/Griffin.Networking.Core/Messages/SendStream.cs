using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Griffin.Networking.Messages
{
    /// <summary>
    /// Send an entire stream
    /// </summary>
    /// <remarks>
    /// Stream will be disposed by the framework
    /// </remarks>
    public class SendStream : IPipelineMessage
    {
        /// <summary>
        /// Gets stream to send
        /// </summary>
        public Stream Stream { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendStream"/> class.
        /// </summary>
        /// <param name="stream">Stream to send, the framework takes ownership.</param>
        public SendStream(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            Stream = stream;
        }
    }
}
