namespace Griffin.Networking.Protocol.FreeSwitch.Events.Sip
{
    [EventName("SOFIA::UNREGISTER", IsCustom = true)]
    public class SofiaUnregister : EventBase
    {
        private string _callId;
        private string _contact;
        private int _expires;

        public string ProfileName { get; set; }

        public string UserName { get; set; }

        public string Domain { get; set; }

        public string Contact
        {
            get { return _contact; }
            set { _contact = value; }
        }

        public string CallId
        {
            get { return _callId; }
            set { _callId = value; }
        }

        public int Expires
        {
            get { return _expires; }
            set { _expires = value; }
        }

        /*
			switch_event_add_header(s_event, SWITCH_STACK_BOTTOM, "profile-name", "%s", profile->name);
			switch_event_add_header(s_event, SWITCH_STACK_BOTTOM, "from-user", "%s", to_user);
			switch_event_add_header(s_event, SWITCH_STACK_BOTTOM, "from-host", "%s", to_host);
			switch_event_add_header(s_event, SWITCH_STACK_BOTTOM, "rpid", "%s", rpid);
        */

        public override bool ParseParameter(string name, string value)
        {
            switch (name)
            {
                case "profile-name":
                    ProfileName = value;
                    break;
                case "from-user":
                    UserName = value;
                    break;
                case "from-host":
                    Domain = value;
                    break;
                case "contact":
                    _contact = value;
                    break;
                case "call-id":
                    _callId = value;
                    break;
                case "expires":
                    int.TryParse(value, out _expires);
                    break;
                default:
                    return base.ParseParameter(name, value);
            }
            return true;
        }

        public override string ToString()
        {
            return "SofiaUnregister(" + UserName + "@" + Domain + ", " + _expires + ")." + base.ToString();
        }
    }
}