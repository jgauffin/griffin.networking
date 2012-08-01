using System;
using System.IO;
using System.Text;
using Griffin.Networking.Messages;

namespace Griffin.Networking.Protocol.FreeSwitch.Net.Handlers
{
    public class ProtocolTrace : IUpstreamHandler, IDownstreamHandler
    {
        private readonly Stream _destinationStream;

        public ProtocolTrace(Stream destinationStream)
        {
            _destinationStream = destinationStream;
        }

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
                var bytes =
                    Encoding.ASCII.GetBytes("\r\nTo FreeSwitch ***************** " + DateTime.Now.ToShortDateString() +
                                            " " +
                                            DateTime.Now.ToString("HH:mm:ss.nnn") + " ******************\r\n\r\n");
                _destinationStream.Write(bytes, 0, bytes.Length);
                _destinationStream.Write(msg.BufferSlice.Buffer, 0, msg.BufferSlice.Count);
                _destinationStream.Flush();
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
                var bytes =
                    Encoding.ASCII.GetBytes("\r\nFrom FreeSwitch***************** " + DateTime.Now.ToShortDateString() +
                                            " " +
                                            DateTime.Now.ToString("HH:mm:ss.nnn") + " ******************\r\n\r\n");
                _destinationStream.Write(bytes, 0, bytes.Length);
                _destinationStream.Write(msg.BufferSlice.Buffer, 0, msg.BufferSlice.Count);
                _destinationStream.Flush();
            }

            context.SendUpstream(message);
        }

        #endregion
    }
}