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
        /// <summary>
        /// Initializes a new instance of the <see cref="TcpServerChildChannel"/> class.
        /// </summary>
        /// <param name="pipeline">The pipeline used to send messages upstream.</param>
        public TcpServerChildChannel(IPipeline pipeline) : base(pipeline)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpServerChildChannel"/> class.
        /// </summary>
        /// <param name="pipeline">The pipeline used to send messages upstream.</param>
        /// <param name="pool">The pool.</param>
        public TcpServerChildChannel(IPipeline pipeline, BufferPool pool) : base(pipeline, pool)
        {
        }

        /// <summary>
        /// Start the channel (by invoking BeginRead)
        /// </summary>
        public void StartChannel()
        {
            StartRead();
        }
    }
}
