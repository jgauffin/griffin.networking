namespace Griffin.Networking.Protocol.FreeSwitch.Events.Channel
{
    /// <summary>
    /// Function have been executed on channel
    /// </summary>
    [EventName("CHANNEL_EXECUTE")]
    public class ChannelExecute : ChannelStateEvent
    {
        /// <summary>
        /// Gets application to execute.
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// Gets arguments for the application
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// Gets response from the application
        /// </summary>
        protected string Response { get; set; }

        /// <summary>
        /// Parse a parameter from FreeSWITCH
        /// </summary>
        /// <param name="name">Parameter name as defined by FS</param>
        /// <param name="value">Parameter value</param>
        /// <returns>
        /// true if parsed sucessfully; otherwise false.
        /// </returns>
        public override bool ParseParameter(string name, string value)
        {
            switch (name)
            {
                case "application":
                    AppName = value;
                    break;
                case "application-data":
                    Arguments = value;
                    break;
                case "Application-Response":
                    Response = value == "_none_" ? null : value;
                    break;
                default:
                    return base.ParseParameter(name, value);
            }

            return true;
        }

        public override string ToString()
        {
            return "ChannelExecute(" + AppName + ", '" + Arguments + "')." + base.ToString();
        }
    }
}