using System;
using System.Collections.Generic;
using System.Linq;
using Griffin.Networking.Logging;
using Griffin.Networking.Protocol.FreeSwitch.Commands;
using Griffin.Networking.Protocol.FreeSwitch.Events.System;
using Griffin.Networking.Protocol.FreeSwitch.Net.Messages;

namespace Griffin.Networking.Protocol.FreeSwitch.Net.Handlers
{
    /// <summary>
    /// Enqueues commands and processes their replies
    /// </summary>
    /// <remarks>Has to be added after the <see cref="EventDecoder"/> if you are using background jobs.</remarks>
    public class CommandDispatcher : IDownstreamHandler, IUpstreamHandler
    {
        private readonly ILogger _logger = LogManager.GetLogger<CommandDispatcher>();
        private readonly Queue<SendCommandMessage> _outbound = new Queue<SendCommandMessage>();
        private readonly LinkedList<BgApiCmd> _pending = new LinkedList<BgApiCmd>();
        private ICommand _current;

        #region IDownstreamHandler Members

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
            if (!(message is SendCommandMessage))
            {
                context.SendDownstream(message);
                return;
            }

            lock (_outbound)
            {
                var cmd = (SendCommandMessage) message;
                if (_current == null)
                {
                    _logger.Trace("Passing command down " + cmd.Command);
                    SendCommand(context, cmd);
                }
                else
                {
                    _logger.Debug("Enqueueing command " + cmd.Command);
                    _outbound.Enqueue(cmd);
                }
            }
        }

        #endregion

        #region IUpstreamHandler Members

        /// <summary>
        /// Handle an message
        /// </summary>
        /// <param name="context">Context unique for this handler instance</param>
        /// <param name="message">Message to process</param>
        /// <remarks>
        /// All messages that can't be handled MUST be send up the chain using <see cref="IPipelineHandlerContext.SendUpstream"/>.
        /// </remarks>
        public void HandleUpstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            if (message is EventRecieved)
            {
                HandleBackgroundCompletion(context, (EventRecieved) message);
                return;
            }
            if (!(message is ReceivedMessage))
            {
                context.SendUpstream(message);
                return;
            }


            var msg = (ReceivedMessage) message;
            var type = msg.Message.Headers["Content-Type"];
            if (type == null || type != "command/reply")
            {
                context.SendUpstream(message);
                return;
            }

            SendCommandMessage nextCommand = null;
            ICommand curMsg;
            lock (_outbound)
            {
                if (_current == null)
                {
                    throw new InvalidOperationException("No active command set.");
                }
                _logger.Trace("Got reply for " + _current);
                curMsg = _current;
                nextCommand = _outbound.Count > 0 ? _outbound.Dequeue() : null;
                _current = null;
            }

            if (curMsg is BgApiCmd)
            {
                HandleBgApi(context, msg.Message, (BgApiCmd) curMsg);
            }
            else
            {
                var result = msg.Message.Headers["Reply-Text"].StartsWith("+");
                var body = msg.Message.Headers["Reply-Text"];
                var reply = new CommandReply(curMsg, result, body);
                _logger.Debug("Sending reply " + body);
                context.SendUpstream(reply);
            }

            if (nextCommand != null)
            {
                _logger.Trace("Sending next command down: " + nextCommand.Command);
                SendCommand(context, nextCommand);
            }
        }

        #endregion

        private void HandleBackgroundCompletion(IPipelineHandlerContext context, EventRecieved message)
        {
            var msg = message.FreeSwitchEvent as BackgroundJob;
            if (msg == null)
            {
                context.SendUpstream(message);
                return;
            }

            BgApiCmd command;
            lock (_pending)
            {
                command = _pending.FirstOrDefault(x => x.Id == msg.JobUid);
                if (command == null)
                {
                    throw new InvalidOperationException("Got a command reply but no pending background job: " + msg);
                }

                _pending.Remove(command);
            }


            context.SendUpstream(new CommandReply(command.Inner, msg.CommandResult.StartsWith("+"), msg.CommandResult));
        }

        private void HandleBgApi(IPipelineHandlerContext context, Message response, BgApiCmd curMsg)
        {
            if (response.Headers["Reply-Text"][0] != '+')
            {
                context.SendUpstream(new CommandReply(curMsg.Inner, false, response.Headers["Reply-Text"]));
                return;
            }

            curMsg.Id = response.Headers["Job-UUID"];
            _pending.AddLast(curMsg);
        }

        private void SendCommand(IPipelineHandlerContext context, SendCommandMessage msg)
        {
            if (msg.Command is AuthCmd || msg.Command is SubscribeOnEvents)
            {
                context.SendDownstream(msg);
                _current = msg.Command;
                return;
            }

            _current = new BgApiCmd(msg.Command);
            var message = new SendCommandMessage(_current);
            context.SendDownstream(message);
        }

        #region Nested type: BgApiCmd

        public class BgApiCmd : ICommand
        {
            private readonly ICommand _inner;

            public BgApiCmd(ICommand inner)
            {
                _inner = inner;
            }

            public string Id { get; set; }

            public ICommand Inner
            {
                get { return _inner; }
            }

            #region ICommand Members

            /// <summary>
            /// Convert command to a string that can be sent to FreeSWITCH
            /// </summary>
            /// <returns>FreeSWITCH command</returns>
            public string ToFreeSwitchString()
            {
                return "bgapi " + _inner.ToFreeSwitchString();
            }

            #endregion
        }

        #endregion
    }
}