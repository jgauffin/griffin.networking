namespace Griffin.Networking.Protocol.FreeSwitch.Events.Channel
{
    [EventName("CHANNEL_EXECUTE_COMPLETE")]
    public class ChannelExecuteComplete : ChannelStateEvent
    {
        public string AppName { get; set; }

        public string Arguments { get; set; }

        /// <summary>
        /// Gets reponse from the application
        /// </summary>
        protected string Response { get; set; }

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
                case "application-response":
                    Response = value == "_none_" ? null : value;
                    break;
                default:
                    return base.ParseParameter(name, value);
            }

            return true;
        }

        public override string ToString()
        {
            return "ExecuteComplete(" + AppName + ", '" + Arguments + "')." + base.ToString();
        }
    }
}