using System.Collections.Specialized;

namespace Griffin.Networking.Protocol.FreeSwitch.Events.Channel
{
    public class ChannelBase : EventBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelBase"/> class.
        /// </summary>
        public ChannelBase()
        {
            Id = null;
            Variables = new NameValueCollection();
        }

        public override bool IsChannelEvent
        {
            get { return true; }
        }

        /// <summary>
        /// Information about the used channel and it's state.
        /// </summary>
        public ChannelInfo ChannelInfo { get; set; }

        public UniqueId Id { get; set; }

        public string AnswerState { get; set; }

        public ChannelDirection CallDirection { get; set; }

        public NameValueCollection Variables { get; private set; }

        public string Protocol { get; set; }

        public ChannelDirection PresenceCallDirection { get; set; }

        public CallState CallState { get; set; }

        public bool HasHitDialplan { get; set; }

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
                case "unique-id":
                    Id = new UniqueId(value);
                    break;
                case "answer-state":
                    AnswerState = value;
                    break;
                case "call-direction":
                    CallDirection = Enumm.Parse<ChannelDirection>(value);
                    break;
                case "channel-call-state":
                    CallState = Enumm.Parse<CallState>(value);
                    break;
                case "presence-call-direction":
                    PresenceCallDirection = Enumm.Parse<ChannelDirection>(value);
                    break;
                case "channel-hit-dialplan":
                    HasHitDialplan = value == "true";
                    break;
                case "proto":
                    Protocol = value;
                    break;

                default:
                    if (name.StartsWith("variable_"))
                    {
                        Variables.Add(name.Remove(0, 10), value);
                        return true;
                    }
                    if (name.Length > 8 && name.Substring(0, 8) == "channel-")
                    {
                        if (ChannelInfo == null)
                            ChannelInfo = new ChannelInfo();
                        return ChannelInfo.Parse(name, value);
                    }

                    return base.ParseParameter(name, value);
            }

            return true;
        }

        public override string ToString()
        {
            return
                "ChannelBase(" + Id + " [" + CallDirection + "] " + AnswerState +
                ", channelInfo{" + ChannelInfo + "})";
        }
    }
}