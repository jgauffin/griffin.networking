using System.IO;
using System.Text;
using Griffin.Networking.Buffers;
using Griffin.Networking.Logging;
using Griffin.Networking.Pipelines;
using Griffin.Networking.Protocol.FreeSwitch.Commands;
using Griffin.Networking.Protocol.FreeSwitch.Net.Messages;

namespace Griffin.Networking.Protocol.FreeSwitch.Net.Handlers
{
    public class MessageEncoder : IDownstreamHandler
    {
        private readonly ILogger _logger = LogManager.GetLogger<MessageEncoder>();

        #region IDownstreamHandler Members

        /// <summary>
        /// Takes a <see cref="Message"/> and converts it into a byte buffer.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="message"></param>
        public void HandleDownstream(IPipelineHandlerContext ctx, IPipelineMessage message)
        {
            if (message is SendCommandMessage)
            {
                var evt = (SendCommandMessage) message;
                var slice = EncodeCommand(evt.Command);
                message = new SendSlice(slice);
            }

            ctx.SendDownstream(message);
        }

        #endregion

        private BufferSlice EncodeCommand(ICommand command)
        {
            /*var str = command is AuthCmd || command is SubscribeOnEvents
                          ? command.ToFreeSwitchString() + "\n\n"
                          : "bgapi " + command.ToFreeSwitchString() + "\n\n";
             * */
            var str = command.ToFreeSwitchString() + "\n\n";

            var cmd = Encoding.ASCII.GetBytes(str);
            _logger.Debug("Encoded: " + str);
            var slice = new BufferSlice(cmd, 0, cmd.Length);
            _logger.Debug("Slice " + slice.Offset + "/" + slice.Count);
            return slice;
        }

        private BufferSlice EncodeMessage(Message msg)
        {
            if (msg.Body.Length != 0)
                msg.Headers["Content-Length"] = msg.Body.Length.ToString();

            var buffer = new byte[65535];
            long length = 0;
            using (var stream = new MemoryStream(buffer))
            {
                stream.SetLength(0);
                using (var writer = new StreamWriter(stream))
                {
                    foreach (string key in msg.Headers)
                    {
                        writer.Write(string.Format("{0}: {1}\n", key, msg.Headers[key]));
                    }
                    writer.Write("\n");


                    writer.Flush();
                    stream.Write(stream.GetBuffer(), 0, (int) stream.Length);
                    length = stream.Length;
                }
            }

            var tmp = Encoding.ASCII.GetString(buffer, 0, (int) length);
            return new BufferSlice(buffer, 0, (int) length);
        }
    }
}