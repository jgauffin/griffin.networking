namespace Griffin.Networking.Protocol.FreeSwitch.Events.System
{
    [EventName("RE_SCHEDULE")]
    public class ReSchedule : EventBase
    {
        private int _taskId;

        public int TaskId
        {
            get { return _taskId; }
            set { _taskId = value; }
        }

        public string Description { get; set; }

        public string Group { get; set; }

        public string Runtime { get; set; }

        public override bool ParseParameter(string name, string value)
        {
            switch (name)
            {
                case "task-id":
                    int.TryParse(value, out _taskId);
                    break;
                case "task-desc":
                    Description = value;
                    break;
                case "task-group":
                    Group = value;
                    break;
                case "task-runtime":
                    Runtime = value;
                    break;
                default:
                    return base.ParseParameter(name, value);
            }

            return true;
        }
    }
}