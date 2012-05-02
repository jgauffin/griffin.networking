using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Networking.Buffers;
using Griffin.Networking.JsonRpc.Messages;

namespace Griffin.Networking.JsonRpc.Handlers
{
    class JsonDecoder : IUpstreamHandler
    {
        private static BufferPool _bufferPool = new BufferPool(65535, 50, 50);
        private SimpleHeader _header;
        private BufferPoolStream _stream;

        public JsonDecoder()
        {
            var slice = _bufferPool.PopSlice();
            _stream = new BufferPoolStream(_bufferPool, slice);
        }

        public void HandleUpstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            var headerMsg = message as ReceivedHeader;
            if (headerMsg != null)
            {
                _header = headerMsg.Header;
                if (_header.Length > 65535)
                {
                    var error = new ErrorResponse("-9999", new RpcError
                                                      {
                                                          Code = RpcErrorCode.InvalidRequest,
                                                          Message = "Support requests which is at most 655355 bytes.",
                                                      });
                    context.SendDownstream(new SendResponse(error));
                    return;
                }


            }


        }
    }
}
