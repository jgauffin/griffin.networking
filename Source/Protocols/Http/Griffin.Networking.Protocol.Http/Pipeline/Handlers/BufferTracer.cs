using System.IO;
using System.Text;
using Griffin.Networking.Logging;
using Griffin.Networking.Pipelines;
using Griffin.Networking.Pipelines.Messages;

namespace Griffin.Networking.Protocol.Http.Handlers
{
    /// <summary>
    /// Writes  content to the log
    /// </summary>
    /// <remarks>
    /// Should be the first up handler and/or the last downstream handler.
    /// </remarks>
    public class BufferTracer : IUpstreamHandler, IDownstreamHandler
    {
        private readonly ILogger _logger = LogManager.GetLogger<BufferTracer>();

        #region IDownstreamHandler Members

        /// <summary>
        /// Process message
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        /// <remarks>
        /// Should always call either <see cref="IPipelineHandlerContext.SendDownstream"/> or <see cref="IPipelineHandlerContext.SendUpstream"/>
        /// unless the handler really wants to stop the processing.
        /// </remarks>
        public void HandleDownstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            var msg = message as SendSlice;
            if (msg != null)
            {
                var stream = new MemoryStream();
                stream.Write(msg.Slice.Buffer, msg.Slice.Offset, msg.Length);

                var reader = new StreamReader(stream);
                var str = reader.ReadToEnd();

                var sb = GetAlphaNumeric(str);
                _logger.Trace(sb.ToString());
            }

            var msg2 = message as SendStream;
            if (msg2 != null)
            {
                var buffer = new byte[msg2.Stream.Length];
                msg2.Stream.Read(buffer, 0, buffer.Length);
                msg2.Stream.Position = 0;
                var str = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                var sb = GetAlphaNumeric(str);
                _logger.Trace(sb.ToString());
            }

            context.SendDownstream(message);
        }

        #endregion

        #region IUpstreamHandler Members

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
            var msg = message as Received;
            if (msg != null)
            {
                var stream = new MemoryStream();
                msg.BufferReader.CopyTo(stream, msg.BufferReader.Count);
                msg.BufferReader.Position = 0;

                var reader = new StreamReader(stream);
                var str = reader.ReadToEnd();
                var sb = GetAlphaNumeric(str);
                _logger.Trace(sb.ToString());
            }

            context.SendUpstream(message);
        }

        #endregion

        private static StringBuilder GetAlphaNumeric(string str)
        {
            var sb = new StringBuilder();
            foreach (var ch in str)
            {
                if (!char.IsSymbol(ch))
                    sb.Append(ch);
            }
            return sb;
        }
    }
}