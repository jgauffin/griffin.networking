namespace Griffin.Networking.Protocol.FreeSwitch.Events.System
{
    /// <summary>
    /// Someone executed a command
    /// </summary>
    [EventName("API")]
    public class ApiEvent : EventBase
    {
        /// <summary>
        /// Gets executed command including parameters.
        /// </summary>
        public string Command { get; private set; }

        protected string Arguments { get; set; }

        public override bool ParseParameter(string name, string value)
        {
            switch (name)
            {
                case "api-command":
                    Command = value;
                    break;
                case "api-command-argument":
                    Arguments = value;
                    break;
                default:
                    return base.ParseParameter(name, value);
            }

            return true;
        }
    }
}