using System;
using System.IO;
using Griffin.Networking.Buffers;

namespace Griffin.Networking.Messages
{
    /// <summary>
    /// Channel received more bytes in the stream
    /// </summary>
    /// <remarks>All streams used by this message support the <see cref="IPeekable"/> interface. </remarks>
    public class ReceivedInStream : IPipelineMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivedInStream"/> class.
        /// </summary>
        /// <param name="stream">Stream that received bytes were written to.</param>
        public ReceivedInStream(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (!(stream is IPeekable))
                throw new ArgumentException("streams used in this message must implement IPeekable.");
            Stream = stream;
        }

        /// <summary>
        /// Gets stream that received bytes were written to
        /// </summary>
        /// <remarks>The stream is owned by the channel, do not dispose it</remarks>
        public Stream Stream { get; private set; }
    }
}