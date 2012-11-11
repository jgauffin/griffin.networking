using System.IO;
using Griffin.Networking.Buffers;
using Griffin.Networking.Pipelines;
using Griffin.Networking.Pipelines.Messages;
using Griffin.Networking.Http.Messages;

namespace Griffin.Networking.Http.Pipeline.Handlers
{
    /// <summary>
    /// Encode message to something that can be sent over the wire.
    /// </summary>
    public class ResponseEncoder : IDownstreamHandler
    {
        readonly static BufferSliceStack _pool = new BufferSliceStack(1000, 65536);


        /// <summary>
        /// Process message
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        public void HandleDownstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            var msg = message as SendHttpResponse;
            if (msg == null)
            {
                context.SendDownstream(message);
                return;
            }

            var slice = _pool.Pop();
            var serializer = new HttpHeaderSerializer();
            var stream = new SliceStream(slice);
            serializer.SerializeResponse(msg.Response, stream);
            context.SendDownstream(new SendSlice(slice, (int) stream.Length));

            if (!msg.Response.KeepAlive)
                context.SendDownstream(new Close());
        }
    }
}