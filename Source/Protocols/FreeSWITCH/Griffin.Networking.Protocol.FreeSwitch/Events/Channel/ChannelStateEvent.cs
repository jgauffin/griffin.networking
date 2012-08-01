namespace Griffin.Networking.Protocol.FreeSwitch.Events.Channel
{
    [EventName("CHANNEL_STATE")]
    public class ChannelStateEvent : ChannelBase
    {
        private const string Yes = "yes";
        private const string CallerTag = "caller-";
        private const string OriginatorTag = "originator-";
        private const string OriginateeTag = "originatee-";
        private const string OtherLegTag = "other-leg-";
        private const string ScreenBitTag = "screen-bit";
        private PartyInfo _caller = PartyInfo.Empty;
        private PartyInfo _originator = PartyInfo.Empty;
        private PartyInfo _otherLeg = PartyInfo.Empty;

        /// <summary>
        /// Information about the called party.
        /// This property is null for certain channel states,
        /// depending on if FreeSwitch sends it or not.
        /// </summary>
        public PartyInfo Caller
        {
            get { return _caller; }
            set { _caller = value; }
        }

        /// <summary>
        /// Information about the destination (place that the caller want to reach).
        /// This property is null for certain channel states,
        /// depending on if FreeSwitch sends it or not.
        /// </summary>
        public PartyInfo Originator
        {
            get { return _originator; }
            set { _originator = value; }
        }

        public CallerId Callee { get; set; }

        public bool ScreenBit { get; set; }

        /// <summary>
        /// Other leg of call.
        /// </summary>
        public PartyInfo OtherLeg
        {
            get { return _otherLeg; }
            set { _otherLeg = value; }
        }

        protected UniqueId CallId { get; set; }

        protected string PresenceId { get; set; }

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
                case ScreenBitTag:
                    ScreenBit = value == Yes;
                    break;
                    /*
                case "channel-presence-id":
                    PresenceId = value;
                    break;
                case "channel-call-uuid":
                    CallId = new UniqueId(value);
                    break;*/
            }

            if (name.StartsWith(OriginateeTag) || name.StartsWith(OriginatorTag))
            {
                if (_originator == PartyInfo.Empty)
                    _originator = new PartyInfo();

                return _originator.Parse(name.Substring(11), value);
            }
            if (name.StartsWith(CallerTag))
            {
                if (_caller == PartyInfo.Empty)
                    _caller = new PartyInfo();

                return _caller.Parse(name.Substring(7), value);
            }
            if (name.StartsWith(OtherLegTag))
            {
                if (_otherLeg == PartyInfo.Empty)
                    _otherLeg = new PartyInfo();

                return _otherLeg.Parse(name.Substring(10), value);
            }


            var res = base.ParseParameter(name, value);
            //          if (!res)
            //                Debugger.Break();

            return res;
        }

        public override string ToString()
        {
            var temp = "ChannelState(";
            if (Caller != PartyInfo.Empty)
                temp += " Caller{" + Caller + "}";
            if (Originator != PartyInfo.Empty)
                temp += " Originator{" + Originator + "}";
            if (OtherLeg != PartyInfo.Empty)
                temp += " OtherLeg{" + OtherLeg + "}";
            return temp + ")." + base.ToString();
        }
    }
}