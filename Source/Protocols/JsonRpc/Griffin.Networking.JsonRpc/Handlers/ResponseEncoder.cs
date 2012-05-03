using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Networking.Buffers;
using Griffin.Networking.JsonRpc.Messages;
using Griffin.Networking.Messages;
using Newtonsoft.Json;

namespace Griffin.Networking.JsonRpc.Handlers
{
    /// <summary>
    /// Encodes <see cref="SendResponse"/> classes into byte[] arrays
    /// </summary>
    public class ResponseEncoder : IDownstreamHandler
    {
        private static readonly BufferPool _bufferPool = new BufferPool(65535, 50, 100);

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
            var msg =  message as SendResponse;
            if (msg == null)
            {
                context.SendDownstream(message);
                return;
            }

            var result = JsonConvert.SerializeObject(msg.Response, Formatting.None);

            // send header
            var header = new byte[5];
            header[0] = 1;
            var lengthBuffer = BitConverter.GetBytes(result.Length);
            Buffer.BlockCopy(lengthBuffer, 0, header, 1, lengthBuffer.Length);
            context.SendDownstream(new SendBuffer(header, 0, 5));

            // send JSON
            var slice = _bufferPool.PopSlice();
            Encoding.UTF8.GetBytes(result, 0, result.Length, slice.Buffer, slice.StartOffset);
            slice.Position = slice.StartOffset;
            slice.Count = result.Length;
            context.SendDownstream(new SendSlice(slice));
        }
    }

}
