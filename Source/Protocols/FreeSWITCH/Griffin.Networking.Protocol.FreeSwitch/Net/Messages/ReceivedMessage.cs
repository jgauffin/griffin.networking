namespace Griffin.Networking.Protocol.FreeSwitch.Net.Messages
{
    public class ReceivedMessage : IPipelineMessage
    {
        public ReceivedMessage(Message message)
        {
            Message = message;
        }

        public Message Message { get; private set; }
    }
}