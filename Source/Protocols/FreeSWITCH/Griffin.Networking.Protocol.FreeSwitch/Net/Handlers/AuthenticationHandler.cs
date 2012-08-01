using System.Security;
using Griffin.Networking.Protocol.FreeSwitch.Commands;
using Griffin.Networking.Protocol.FreeSwitch.Net.Messages;

namespace Griffin.Networking.Protocol.FreeSwitch.Net.Handlers
{
    public class AuthenticationHandler : IUpstreamHandler
    {
        public AuthenticationHandler(SecureString password)
        {
            Password = password;
        }

        public SecureString Password { get; private set; }

        #region IUpstreamHandler Members

        public void HandleUpstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            var msg = message as ReceivedMessage;
            if (msg != null && msg.Message.Headers["Content-Type"] == "auth/request")
            {
                var api = new AuthCmd(Password);
                context.SendDownstream(new SendCommandMessage(api));
                return;
            }

            var reply = message as CommandReply;
            if (reply != null && reply.OriginalCommand is AuthCmd)
            {
                if (!reply.IsSuccessful)
                    context.SendUpstream(new AuthenticationFailed(reply.Body));
                else
                    context.SendUpstream(new Authenticated());

                return;
            }

            context.SendUpstream(message);
        }

        #endregion
    }
}