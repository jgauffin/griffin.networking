using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace Griffin.Networking.Tests.Channels
{
    public class MyPipeline : IPipeline
    {
        public List<IPipelineMessage> DownstreamMessages2 = new List<IPipelineMessage>();
        public List<IPipelineMessage> UpstreamMessages = new List<IPipelineMessage>();
        ManualResetEvent _upstreamEvent = new ManualResetEvent(false);
        ManualResetEvent _downstreamEvent = new ManualResetEvent(false);
        private Type _upstreamTypeToWaitOn;
        private Type _downstreamTypeToWaitOn;

        public void SendUpstream(IPipelineMessage message)
        {
            UpstreamMessages.Add(message);
            if (_upstreamTypeToWaitOn != null && _upstreamTypeToWaitOn.IsAssignableFrom(message.GetType()))
                _upstreamEvent.Set();
        }

        public void SetChannel(IChannel channel)
        {

        }

        public void SendDownstream(IPipelineMessage message)
        {
            DownstreamMessages2.Add(message);
            if (_downstreamTypeToWaitOn != null && _downstreamTypeToWaitOn.IsAssignableFrom(message.GetType()))
                _downstreamEvent.Set();
        }

        public bool WaitOnUpstream<T>(TimeSpan timeSpan) where T : IPipelineMessage
        {
            _upstreamTypeToWaitOn = typeof (T);
            return UpstreamMessages.Any(t => _upstreamTypeToWaitOn.IsAssignableFrom(t.GetType())) ||
                   _upstreamEvent.WaitOne(timeSpan);
        }
        public bool WaitOnDownstream<T>(TimeSpan timeSpan) where T : IPipelineMessage
        {
            _downstreamTypeToWaitOn = typeof(T);
            return DownstreamMessages2.Any(t => _downstreamTypeToWaitOn.IsAssignableFrom(t.GetType())) ||
                   _downstreamEvent.WaitOne(timeSpan);
        }
    }
}