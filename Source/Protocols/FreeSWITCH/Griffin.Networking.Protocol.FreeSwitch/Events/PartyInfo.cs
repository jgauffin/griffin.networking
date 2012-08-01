using System;

namespace Griffin.Networking.Protocol.FreeSwitch.Events
{
    public class PartyInfo
    {
        public static PartyInfo Empty = new PartyInfo();
        private string _calleeIdName;
        private string _calleeIdNumber;
        private string _callerIdName = "";
        private string _callerIdNumber = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="PartyInfo"/> class.
        /// </summary>
        public PartyInfo()
        {
            CalleeId = CallerId.Empty;
            Context = string.Empty;
            DestinationNumber = string.Empty;
            CallerId = CallerId.Empty;
            DialplanType = string.Empty;
            ChannelInfo = ChannelName.Empty;
        }

        /// <summary>
        /// Gets or sets username if caller is registered in the switch.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets type of dial plan executing.
        /// </summary>
        public string DialplanType { get; set; }

        /// <summary>
        /// Gets or sets calelr id name.
        /// </summary>
        public CallerId CallerId { get; private set; }

        /// <summary>
        /// Gets or sets called destination.
        /// </summary>
        public string DestinationNumber { get; set; }

        /// <summary>
        /// Gets or sets end point ip
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets ani.
        /// </summary>
        public string ANI { get; set; }

        /// <summary>
        /// Gets or sets unique call id.
        /// </summary>
        public UniqueId Id { get; set; }

        /// <summary>
        /// Gets or sets dial plan context.
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// Gets or sets channel name.
        /// </summary>
        public ChannelName ChannelInfo { get; set; }

        /// <summary>
        /// Gets or sets when profile was created.
        /// </summary>
        public DateTime ProfileCreatedAt { get; set; }

        /// <summary>
        /// Gets or sets when channel was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets when channel was answered.
        /// </summary>
        public DateTime AnsweredAt { get; set; }

        /// <summary>
        /// Gets or sets when channel was hung up.
        /// </summary>
        public DateTime HangupAt { get; set; }

        /// <summary>
        /// Gets or sets when channel was transferred.
        /// </summary>
        public DateTime TransferredAt { get; set; }


        protected bool PrivacyHideNumber { get; set; }

        protected bool PrivacyHideName { get; set; }

        protected bool ScreenBit { get; set; }

        public CallerId CalleeId { get; private set; }

        protected long ChannelProgressMediaTime { get; set; }

        protected long ChannelProgressTime { get; set; }

        protected int ProfileIndex { get; set; }

        /// <summary>
        /// Gets source (as "mod_sofia")
        /// </summary>
        protected string EndpointSource { get; set; }

        protected ChannelDirection Direction { get; set; }

        public bool Parse(string name, string value)
        {
            switch (name)
            {
                case "username":
                    UserName = value;
                    break;
                case "dialplan":
                    DialplanType = value;
                    break;
                case "caller-id-name":
                    _callerIdName = value;
                    CreateCallerId();
                    break;
                case "caller-id-number":
                    _callerIdNumber = value;
                    CreateCallerId();
                    break;
                case "callee-id-name":
                    _calleeIdName = value;
                    CreateCalleeId();
                    break;
                case "callee-id-number":
                    _calleeIdNumber = value;
                    CreateCalleeId();
                    break;
                case "network-addr":
                    IpAddress = value;
                    break;
                case "ani":
                    ANI = value;
                    break;
                case "destination-number":
                    DestinationNumber = value;
                    break;
                case "unique-id":
                    Id = new UniqueId(value);
                    break;
                case "context":
                    Context = value;
                    break;
                case "channel-name":
                    ChannelInfo = new ChannelName();
                    ChannelInfo.Parse(value);
                    break;
                case "profile-created-time":
                    ProfileCreatedAt = value.FromUnixTime();
                    break;
                case "channel-created-time":
                    CreatedAt = value.FromUnixTime();
                    break;
                case "channel-answered-time":
                    AnsweredAt = value.FromUnixTime();
                    break;
                case "channel-hangup-time":
                    HangupAt = value.FromUnixTime();
                    break;
                case "channel-transfer-time":
                    TransferredAt = value.FromUnixTime();
                    break;
                case "screen-bit":
                    ScreenBit = value == "yes";
                    break;
                case "privacy-hide-name":
                    PrivacyHideName = value == "yes";
                    break;
                case "privacy-hide-number":
                    PrivacyHideNumber = value == "yes";
                    break;

                case "direction":
                    Direction = Enumm.Parse<ChannelDirection>(value);
                    break;

                case "source":
                    EndpointSource = value;
                    break;

                case "profile-index":
                    ProfileIndex = int.Parse(value);
                    break;

                case "channel-progress-time":
                    ChannelProgressTime = long.Parse(value);
                    break;
                case "channel-progress-media-time":
                    ChannelProgressMediaTime = long.Parse(value);
                    break;

                default:
                    return false;
            }
            return true;
        }

        private void CreateCalleeId()
        {
            if (_calleeIdName == null || _calleeIdNumber == null)
                return;


            CalleeId = new CallerId(_calleeIdName, new PlainNumber(_calleeIdNumber));
        }

        private void CreateCallerId()
        {
            if (_callerIdName == null || _callerIdNumber == null)
                return;


            CallerId = new CallerId(_callerIdName, new PlainNumber(_callerIdNumber));
        }

        public override string ToString()
        {
            if (CallerId == CallerId.Empty)
            {
                if (Id == null)
                    return "Empty";

                return ChannelInfo + "  " + Id + " --> destination: " + DestinationNumber;
            }

            return ChannelInfo + "(" + CallerId + ") " + Id + " --> destination: " +
                   DestinationNumber;
        }
    }

    /*
    Caller-Username: 23702
    Caller-Dialplan: XML
    Caller-Caller-ID-Name: 23702
    Caller-Caller-ID-Number: 23702
    Caller-Network-Addr: 192.168.2.5
    Caller-ANI: 23702
    Caller-Destination-Number: 2000
    Caller-Unique-ID: 482c78ba-a2bf-4324-bf09-388b7b5fbb54
    Caller-Source: mod_sofia
    Caller-Context: internal
    Caller-Channel-Name: sofia/internal/23702@domain23702.com
    Caller-Profile-Index: 1
    Caller-Profile-Created-Time: 1266851737825925
    Caller-Channel-Created-Time: 1266851737825925
    Caller-Channel-Answered-Time: 0
    Caller-Channel-Progress-Time: 0
    Caller-Channel-Progress-Media-Time: 0
    Caller-Channel-Hangup-Time: 0
    Caller-Channel-Transfer-Time: 0
    Caller-Screen-Bit: true
    Caller-Privacy-Hide-Name: false
    Caller-Privacy-Hide-Number: false
         * */
}