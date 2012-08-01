namespace Griffin.Networking.Protocol.FreeSwitch.Events.Channel
{
    [EventName("DTMF")]
    public class Dtmf : ChannelBase
    {
        private int _duration;

        public char Digit { get; set; }

        public int Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        public override bool ParseParameter(string name, string value)
        {
            switch (name)
            {
                case "dtmf-digit":
                    Digit = value[0];
                    break;
                case "dtmf-duration":
                    int.TryParse(value, out _duration);
                    break;
            }

            return base.ParseParameter(name, value);
        }

        public override string ToString()
        {
            return "Dtmf(" + Digit + ")." + base.ToString();
        }
    }
}