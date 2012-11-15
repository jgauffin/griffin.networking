using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Griffin.Networking.Logging;
using Griffin.Networking.Pipelines;

namespace Griffin.Networking.Tests.Channels
{
    public class MyPipeline : IPipeline
    {
        private readonly ManualResetEvent _downstreamEvent = new ManualResetEvent(false);
        private readonly ILogger _logger = LogManager.GetLogger<MyPipeline>();
        private readonly List<Observer> _upstreamers = new List<Observer>();
        public List<IPipelineMessage> DownstreamMessages2 = new List<IPipelineMessage>();
        public List<IPipelineMessage> UpstreamMessages = new List<IPipelineMessage>();
        private Type _downstreamTypeToWaitOn;

        #region IPipeline Members

        public void SendUpstream(IPipelineMessage message)
        {
            UpstreamMessages.Add(message);
            _logger.Debug("Received: " + message);
            var waiters = _upstreamers.Where(x => x._requestedType == message.GetType()).ToList();
            foreach (var observer in waiters)
            {
                _logger.Trace("Trigering observer: " + observer);
                observer.Trigger(message);
            }
            _upstreamers.RemoveAll(waiters.Contains);
        }

        public void SetChannel(IDownstreamHandler handler)
        {
        }

        public void SendDownstream(IPipelineMessage message)
        {
            DownstreamMessages2.Add(message);
            if (_downstreamTypeToWaitOn != null && _downstreamTypeToWaitOn.IsInstanceOfType(message))
                _downstreamEvent.Set();
        }

        #endregion

        private void AddUpStreamObserver(Type type, Action<IPipelineMessage> callback)
        {
            var msg = UpstreamMessages.FirstOrDefault(type.IsInstanceOfType);
            if (msg != null)
            {
                _logger.Debug("already got msg: " + msg);
                callback(msg);
                return;
            }

            _upstreamers.Add(new Observer {_requestedType = type, Trigger = callback});
        }

        public bool WaitOnUpstream<T>(TimeSpan timeSpan, Action<T> action = null) where T : class, IPipelineMessage
        {
            var evt = new ManualResetEvent(false);
            AddUpStreamObserver(typeof (T), msg =>
                {
                    _logger.Trace("Triggering action");
                    if (action != null)
                        action((T) msg);
                    evt.Set();
                });

            return evt.WaitOne(timeSpan);
        }


        public bool WaitOnDownstream<T>(TimeSpan timeSpan) where T : IPipelineMessage
        {
            _downstreamTypeToWaitOn = typeof (T);
            return DownstreamMessages2.Any(t => _downstreamTypeToWaitOn.IsAssignableFrom(t.GetType())) ||
                   _downstreamEvent.WaitOne(timeSpan);
        }

        #region Nested type: Observer

        public class Observer
        {
            public Action<IPipelineMessage> Trigger;
            public Type _requestedType;
        }

        #endregion
    }
}