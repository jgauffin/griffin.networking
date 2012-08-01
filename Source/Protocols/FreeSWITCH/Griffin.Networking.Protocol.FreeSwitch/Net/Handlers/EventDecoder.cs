using System;
using System.Collections.Specialized;
using System.IO;
using Griffin.Networking.Logging;
using Griffin.Networking.Protocol.FreeSwitch.Events;
using Griffin.Networking.Protocol.FreeSwitch.Net.Messages;

namespace Griffin.Networking.Protocol.FreeSwitch.Net.Handlers
{
    /// <summary>
    /// Parses all <see cref="Message"/> objects that got Content-Type "Event". 
    /// </summary>
    /// <remarks>
    /// <para>This class must be added after <see cref="MessageDecoder"/> in the <see cref="IPipeline"/>.</para>
    /// <para>Note that events which has an own body will get a parameter called <c>__content__</c> which contains the content.</para>
    /// </remarks>
    public class EventDecoder : IUpstreamHandler
    {
        private readonly EventFactory _factory;
        private readonly ILogger _logger = LogManager.GetLogger<EventDecoder>();

        public EventDecoder(EventFactory factory)
        {
            _factory = factory;
        }

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
            var msg = message as ReceivedMessage;
            if (msg == null || msg.Message.Headers["Content-Type"] != "text/event-plain")
            {
                context.SendUpstream(message);
                return;
            }


            var body = ParseBody(msg.Message);
            var fsEvent = CreateEvent(message, body);
            if (fsEvent == null)
            {
                using (var reader = new StreamReader(msg.Message.Body))
                {
                    msg.Message.Body.Position = 0;
                    Console.WriteLine(reader.ReadToEnd());
                }
                context.SendUpstream(message);
                return;
            }

            try
            {
                _logger.Debug("Parsing event " + fsEvent.GetType().Name);
                fsEvent.Parse(body);
                context.SendUpstream(new EventRecieved(fsEvent));
            }
            catch (Exception err)
            {
                msg.Message.Body.Position = 0;
                var reader = new StreamReader(msg.Message.Body);
                throw new InvalidOperationException("Failed to parse event body.\r\n" + reader.ReadToEnd(), err);
            }
        }

        #endregion

        protected virtual NameValueCollection ParseBody(Message msg)
        {
            var col = new NameValueCollection();
            var reader = new StreamReader(msg.Body);
            var line = "";
            try
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (line == "")
                    {
                        var contentLength = col["Content-Length"];
                        if (!string.IsNullOrEmpty(contentLength))
                        {
                            var content = new char[int.Parse(contentLength)];
                            reader.Read(content, 0, content.Length);
                            col.Add("__content__", new string(content));
                        }

                        break;
                        
                    }

                    var pos = line.IndexOf(":", System.StringComparison.Ordinal);
                    if (pos == -1)
                        throw new FormatException("Line '" + line + "' do not contain a colon as we expected.");

                    var key = line.Substring(0, pos).Trim();
                    var value = Uri.UnescapeDataString(line.Substring(pos + 1).Trim());
                    col.Add(key, value);
                }
            }
            catch (Exception err)
            {
                if (err is FormatException)
                    throw; //rethrow our inner exception

                _logger.Error("Failed to decode line: " + line, err);
                throw new FormatException("Failed to decode line " + line, err);
            }
            return col;
        }

        protected virtual EventBase CreateEvent(IPipelineMessage message, NameValueCollection msg)
        {
            var name = String.Compare(msg["Event-Name"], "custom", StringComparison.OrdinalIgnoreCase) == 0
                           ? msg["Event-Subclass"]
                           : msg["Event-Name"];
            if (string.IsNullOrEmpty(name))
            {
                _logger.Warning("Failed to create event " + name);
                return null;
            }

            return msg["Event-Subclass"] == null ? _factory.Create(name) : _factory.CreateCustom(name);
        }
    }
}