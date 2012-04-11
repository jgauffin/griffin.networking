using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Griffin.Networking;
using Griffin.Networking.Messages;

namespace Griffin.Networking.Channels
{
    /// <summary>
    /// A TCP channel used to connect to a remote end point
    /// </summary>
    public class TcpClientChannel : TcpChannel
    {
        private bool _firstTimeConnect = true;
        
        public TcpClientChannel(IPipeline pipeline) : base(pipeline)
        {
        }

        public override void HandleDownstream(IPipelineMessage message)
        {
            if (message is Connect)
                Connect((IPEndPoint) ((Connect) message).RemoteEndPoint);
            else
                base.HandleDownstream(message);
        }

        /// <summary>
        /// Connect to a client
        /// </summary>
        /// <param name="remoteEndPoint"></param>
        public void Connect(IPEndPoint remoteEndPoint)
        {
            try
            {
                Logger.Debug("Connecting to " + remoteEndPoint);
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(remoteEndPoint);
                AssignSocket(socket);
                Pipeline.SendUpstream(new Connected(remoteEndPoint));
                StartRead();
            }
            catch(SocketException err)
            {
                if (_firstTimeConnect)
                    Pipeline.SendUpstream(new PipelineFailure(err));

                _firstTimeConnect = false;
            }
            catch(Exception err)
            {
                Logger.Warning("Failed to connect to " + remoteEndPoint, err);
                Pipeline.SendUpstream(new PipelineFailure(err));
            }
        }

        
    }
}
