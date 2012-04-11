using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Networking.Buffers;

namespace Griffin.Networking.Channels
{
    /// <summary>
    /// A channel for a connected client in a server.
    /// </summary>
    public class TcpServerChildChannel : TcpChannel
    {
        public TcpServerChildChannel(IPipeline pipeline) : base(pipeline)
        {
        }

        public TcpServerChildChannel(IPipeline pipeline, BufferPool pool) : base(pipeline, pool)
        {
        }

        public void StartChannel()
        {
            StartRead();
        }
    }
}
