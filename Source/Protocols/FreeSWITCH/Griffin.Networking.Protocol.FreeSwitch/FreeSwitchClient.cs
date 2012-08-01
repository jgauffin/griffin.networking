using System;
using System.Net;
using System.Security;
using Griffin.Networking.Channels;
using Griffin.Networking.Messages;
using Griffin.Networking.Protocol.FreeSwitch.Commands;
using Griffin.Networking.Protocol.FreeSwitch.Net.Handlers;
using Griffin.Networking.Protocol.FreeSwitch.Net.Messages;

namespace Griffin.Networking.Protocol.FreeSwitch
{
    /// <summary>
    /// The FreeSwitch client.
    /// </summary>
    public class FreeSwitchClient : IUpstreamHandler
    {
        private readonly FreeSwitchEventCollection _eventCollection;
        private readonly IPipeline _pipeline;
        private readonly AsyncJobQueue _waitingObjects;
        private TcpClientChannel _channel;

        public FreeSwitchClient(SecureString password, FreeSwitchEventCollection eventCollection)
        {
            _eventCollection = eventCollection;
            var pipelineFactory = new FreeSwitchPipeline(password, this);
            _pipeline = pipelineFactory.Build();
            _channel = new TcpClientChannel(_pipeline);
            _waitingObjects = new AsyncJobQueue();
        }

        /// <summary>
        /// Gets if the socket has been connected and authenticated.
        /// </summary>
        public bool IsAuthenticated { get; private set; }

        /// <summary>
        /// Gets if socket is connected
        /// </summary>
        /// <remarks>
        /// Client is not fully functional until the socket has been authenticated.
        /// </remarks>
        /// <seealso cref="IsAuthenticated"/>
        public bool IsConnected { get; private set; }

        #region IUpstreamHandler Members

        /// <summary>
        /// Handle an message
        /// </summary>
        /// <param name="context">Context unique for this handler instance</param>
        /// <param name="message">Message to process</param>
        void IUpstreamHandler.HandleUpstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            if (message is Connected)
                IsConnected = true;
            if (message is Disconnected)
            {
                OnDisconnect();
            }
            if (message is Authenticated)
            {
                IsAuthenticated = true;
                _pipeline.SendDownstream(
                    new SendCommandMessage(new SubscribeOnEvents(EventSubscriptionType.Plain, _eventCollection)));
            }
            if (message is CommandReply)
            {
                var reply = (CommandReply) message;
                _waitingObjects.Trigger(reply.OriginalCommand, reply);

                if (reply.OriginalCommand is SubscribeOnEvents)
                {
                    Initialized(this, EventArgs.Empty);                    
                }
            }
            if (message is EventRecieved)
            {
                var msg = (EventRecieved) message;
                EventReceived(this, new EventRecievedEventArgs(msg.FreeSwitchEvent));
            }
        }

        #endregion

        private void OnDisconnect()
        {
            IsConnected = false;
            IsAuthenticated = false;
            Disconnected(this, EventArgs.Empty);
            _waitingObjects.Clear();
        }

        /// <summary>
        /// Connect to freeswitch.
        /// </summary>
        /// <param name="endPoint">FreeSWITCH endpoint.</param>
        public void Connect(IPEndPoint endPoint)
        {
            _pipeline.SendDownstream(new Connect(endPoint));
        }

        /// <summary>
        /// Send a command.
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <param name="callback">Callback method</param>
        /// <param name="state">Any state object you'll like to use</param>
        /// <remarks>The commands are sent using the regulat "api" command. You can invoke as many commands are you
        /// like, but they are queued up and executed in sequence. Use BeginBackgroundCommand if you want to perform
        /// multiple commands in FreeSwitch.</remarks>
        public IAsyncResult BeginSendCommand(ICommand command, AsyncCallback callback, object state)
        {
            var result = _waitingObjects.Enqueue(command, callback, state);
            _pipeline.SendDownstream(new SendCommandMessage(command));
            return result;
        }

        /// <summary>
        /// Get result for a command
        /// </summary>
        /// <param name="result">Async result returned by <see cref="BeginSendCommand"/></param>
        /// <returns>Command result</returns>
        /// <remarks>
        /// Will wait until the command has completed (if it has not completed already).
        /// </remarks>
        public ICommandReply EndSendCommand(IAsyncResult result)
        {
            var asyncRes = (IJobAsyncResult) result;
            result.AsyncWaitHandle.WaitOne();
            var reply = (CommandReply) asyncRes.Result;
            return reply;
        }

        /// <summary>
        /// Received an event from FreeSwitch
        /// </summary>
        public event EventHandler<EventRecievedEventArgs> EventReceived = delegate { };

        /// <summary>
        /// Client is connected and authenticated
        /// </summary>
        public event EventHandler Initialized = delegate { };

        /// <summary>
        /// Client has been disconnected from FreeSWITCH.
        /// </summary>
        public event EventHandler Disconnected = delegate { };
    }
}