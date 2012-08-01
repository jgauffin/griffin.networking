namespace Griffin.Networking.Protocol.FreeSwitch.Events.System
{
    /// <summary>
    /// Heartbeat with current FreeSwitch status
    /// </summary>
    [EventName("HEARTBEAT")]
    public class Heartbeat : EventBase
    {
        private double _idleCpu;
        private int _maxSessionCount;
        private int _sessionCount;
        private int _sessionsPerSecond;
        private int _sessionsSinceStartup;

        public string Info { get; set; }

        public string UpTime { get; set; }

        public int SessionCount
        {
            get { return _sessionCount; }
            set { _sessionCount = value; }
        }

        public override bool ParseParameter(string name, string value)
        {
            switch (name)
            {
                case "event-info":
                    Info = value;
                    break;
                case "up-time":
                    UpTime = value;
                    break;
                case "session-count":
                    int.TryParse(value, out _sessionCount);
                    break;
                case "max-sessions":
                    int.TryParse(value, out _maxSessionCount);
                    break;
                case "session-per-sec":
                    int.TryParse(value, out _sessionsPerSecond);
                    break;
                case "session-since-startup":
                    int.TryParse(value, out _sessionsSinceStartup);
                    break;
                case "idle-cpu":
                    double.TryParse(value, out _idleCpu);
                    break;

                default:
                    return base.ParseParameter(name, value);
            }

            return true;
        }

        public override string ToString()
        {
            return string.Format("Hearbeat(upTime: {0}, sessionCount: {1})", UpTime, SessionCount);
        }
    }
}