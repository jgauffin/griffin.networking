using System;
using System.Net;
using System.Threading;
using Griffin.Networking.Messages;

namespace Griffin.Networking.Protocol.FreeSwitch.Net.Handlers
{
    /// <summary>
    /// Reconnects a client if it has been disconnected
    /// </summary>
    /// <remarks>Hooks down stream to be able to get the IP address to connect to.</remarks>
    public class ReconnectHandler : IUpstreamHandler, IDownstreamHandler
    {
        private readonly TimeSpan _interval;
        private IPEndPoint _endPoint;
        private bool _forcedDisconnect;
        private Timer _timer;
        private IPipelineHandlerContext _upstreamContext;

        public ReconnectHandler(TimeSpan interval)
        {
            _interval = interval;
        }

        #region IDownstreamHandler Members

        public void HandleDownstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            if (message is Connect)
            {
                _endPoint = (IPEndPoint) ((Connect) message).RemoteEndPoint;
                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }
                _forcedDisconnect = false;
            }
            if (message is Disconnect)
            {
                _forcedDisconnect = true;
            }
        }

        #endregion

        #region IUpstreamHandler Members

        public void HandleUpstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            _upstreamContext = context;

            if (message is Connected)
            {
                var msg = (Connected) message;
                _endPoint = (IPEndPoint) msg.RemoteEndPoint;
            }

            if (message is Disconnected)
            {
                if (!_forcedDisconnect)
                {
                    if (_timer == null)
                        _timer = new Timer(OnReConnect, null, Timeout.Infinite, Timeout.Infinite);

                    _timer.Change((int) _interval.TotalMilliseconds, Timeout.Infinite);
                }
            }
        }

        #endregion

        private void OnReConnect(object state)
        {
            _upstreamContext.SendDownstream(new Connect(_endPoint));
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }
}