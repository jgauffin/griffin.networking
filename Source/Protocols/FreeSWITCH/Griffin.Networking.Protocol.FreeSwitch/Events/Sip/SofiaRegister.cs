namespace Griffin.Networking.Protocol.FreeSwitch.Events.Sip
{
    /// <summary>
    /// Sofia_reg.c way down in sofia_reg_handle_register
    /// </summary>
    [EventName("SOFIA::REGISTER", IsCustom = true)]
    public class SofiaRegister : EventBase
    {
        private int _expires;

        public string ProfileName { get; set; }

        public string UserName { get; set; }

        public string Domain { get; set; }

        public string Contact { get; set; }

        public string CallId { get; set; }

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
                    Contact = value;
                    break;
                case "call-id":
                    CallId = value;
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
            return "SofiaRegister(" + UserName + "@" + Domain + ", " + _expires + ")." + base.ToString();
        }
    }
}