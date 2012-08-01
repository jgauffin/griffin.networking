using Griffin.Networking.Protocol.FreeSwitch.Commands;

namespace Griffin.Networking.Protocol.FreeSwitch.Net.Messages
{
    /// <summary>
    /// Reply to a FreeSWITCH command
    /// </summary>
    public class CommandReply : IPipelineMessage, ICommandReply
    {
        public CommandReply(ICommand replyTo, bool result, string message)
        {
            OriginalCommand = replyTo;
            IsSuccessful = result;
            Body = message;
        }

        public ICommand OriginalCommand { get; private set; }

        #region ICommandReply Members

        public bool IsSuccessful { get; private set; }
        public string Body { get; private set; }

        #endregion
    }
}