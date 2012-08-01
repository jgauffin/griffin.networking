namespace Griffin.Networking.Protocol.FreeSwitch.Events.Messaging
{
    /// <summary>
    /// Ask if a user have waiting messages.
    /// </summary>
    [EventName("MESSAGE_QUERY")]
    public class MessageQuery : EventBase
    {
        /// <summary>
        /// Gets account (userName@profile) being queried
        /// </summary>
        public string Account { get; set; }

        public override bool ParseParameter(string name, string value)
        {
            if (name == "message-account")
                Account = value;
            else
                return base.ParseParameter(name, value);

            return true;
        }
    }
}