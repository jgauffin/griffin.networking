using System.Linq;

namespace Griffin.Networking.Protocol.FreeSwitch.Events
{
    public class ChannelInfo : ChannelName
    {
        private string _domainName = string.Empty;
        private int _readCodecRate;
        private int _stateNumber;
        private int _writeCodecRate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelInfo"/> class.
        /// </summary>
        public ChannelInfo()
        {
            ProfileName = string.Empty;
            State = ChannelState.Unknown;
        }

        /// <summary>
        /// Coded used to read information from the channel.
        /// Example: L16
        /// </summary>
        public string ReadCodecName { get; set; }

        /// <summary>
        /// Bitrate of the read coded that is used.
        /// Example: 8000
        /// </summary>
        public int ReadCodecRate
        {
            get { return _readCodecRate; }
            set { _readCodecRate = value; }
        }

        /// <summary>
        /// Codec used for writing to the channel.
        /// Example: L16
        /// </summary>
        public string WriteCodecName { get; set; }

        /// <summary>
        /// Bitrate of the codec.
        /// Example: 8000
        /// </summary>
        public int WriteCodecRate
        {
            get { return _writeCodecRate; }
            set { _writeCodecRate = value; }
        }

        /// <summary>
        /// State that the channel is in. Check ChannelState enum for more information.
        /// </summary>
        public ChannelState State { get; set; }

        /// <summary>
        /// Number of the state (from the enum in FreeSwitch)
        /// </summary>
        public int StateNumber
        {
            get { return _stateNumber; }
            set { _stateNumber = value; }
        }

        /// <summary>
        /// Name of the channel.
        /// </summary>
        /// <example>sofia/mydomain.com/1234@conference.freeswitch.org</example>
        /// <seealso cref="ChannelInfo.Protocol"/>
        /// <seealso cref="ChannelName.EndpointTypeName"/>
        /// <seealso cref="ChannelName.UserName"/>
        /// <seealso cref="ChannelName.DomainName"/>
        public string Name
        {
            get { return base.ToString(); }
            set { Parse(value); }
        }

        protected CallState CallState { get; set; }

        protected int[] WriteCodecBitRate { get; set; }

        protected int[] ReadCodecBitRate { get; set; }

        protected UniqueId CallId { get; set; }

        protected string PresenceId { get; set; }

        public bool Parse(string name, string value)
        {
            switch (name)
            {
                case "channel-state":
                    State = ChannelStateParser.Parse(value);
                    break;
                case "channel-state-number":
                    int.TryParse(value, out _stateNumber);
                    break;
                case "channel-name":
                    Parse(value);
                    break;
                case "channel-read-codec-name":
                    ReadCodecName = value;
                    break;
                case "channel-read-codec-rate":
                    int.TryParse(value, out _readCodecRate);
                    break;
                case "channel-write-codec-name":
                    WriteCodecName = value;
                    break;
                case "channel-write-codec-rate":
                    int.TryParse(value, out _writeCodecRate);
                    break;

                case "channel-presence-id":
                    PresenceId = value;
                    break;

                case "channel-call-uuid":
                    CallId = new UniqueId(value);
                    break;

                case "channel-call-state":
                    CallState = Enumm.Parse<CallState>(value);
                    break;

                case "channel-read-codec-bit-rate":
                    ReadCodecBitRate = value.Split(',').Select(int.Parse).ToArray();
                    break;
                case "channel-write-codec-bit-rate":
                    WriteCodecBitRate = value.Split(',').Select(int.Parse).ToArray();
                    break;

                default:
                    return false;
            }
            return true;
        }

        public static string StateToString(ChannelState state)
        {
            var stateStr = state.ToString();
            var temp = "CS_" + char.ToUpper(stateStr[0]);
            for (var i = 1; i < stateStr.Length; i++)
            {
                if (char.IsUpper(stateStr[i]))
                    temp += stateStr[i] + "_";
                else
                    temp += char.ToUpper(stateStr[i]);
            }

            return temp;
        }

        public override string ToString()
        {
            return base.ToString() + ": " + State;
        }
    }
}