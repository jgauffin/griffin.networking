namespace Griffin.Networking.Protocol.FreeSwitch.Events.Sip
{
    public class SipEvent : EventBase
    {
        private string _from;
        private string _protocol;

        public string Protocol
        {
            get { return _protocol; }
            set { _protocol = value; }
        }

        /// <summary>
        /// username@domain
        /// </summary>
        public string From
        {
            get { return _from; }
            set { _from = value; }
        }

        public override bool ParseParameter(string name, string value)
        {
            switch (name)
            {
                case "from":
                    _from = value;
                    break;
                case "proto":
                    _protocol = value;
                    break;
                default:
                    return base.ParseParameter(name, value);
            }
            return true;
        }

        public override string ToString()
        {
            return "Sip(" + _from + ")." + base.ToString();
        }
    }
}