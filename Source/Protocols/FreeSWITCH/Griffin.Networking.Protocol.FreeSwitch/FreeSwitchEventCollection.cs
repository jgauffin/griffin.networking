using System.Collections.Generic;

namespace Griffin.Networking.Protocol.FreeSwitch
{
    public class FreeSwitchEventCollection
    {
        private readonly IList<FreeSwitchEvent> _events = new List<FreeSwitchEvent>();

        public FreeSwitchEventCollection()
        {
        }

        public FreeSwitchEventCollection(params FreeSwitchEvent[] parameters)
        {
            foreach (var e in parameters)
            {
                Add(e);
            }
        }

        public FreeSwitchEvent this[int index]
        {
            get { return _events[index]; }
        }

        public int Count
        {
            get { return _events.Count; }
        }

        public void Add(FreeSwitchEvent e)
        {
            _events.Add(e);
        }

        public void Clear()
        {
            _events.Clear();
        }

        public static FreeSwitchEventCollection GetChannelEvents()
        {
            var e = new FreeSwitchEventCollection();
            e.Add(FreeSwitchEvent.ChannelAnswer);
            e.Add(FreeSwitchEvent.ChannelBridge);
            e.Add(FreeSwitchEvent.ChannelCreate);
            e.Add(FreeSwitchEvent.ChannelDestroy);
            e.Add(FreeSwitchEvent.ChannelExecute);
            e.Add(FreeSwitchEvent.ChannelHangup);
            e.Add(FreeSwitchEvent.ChannelOutgoing);
            e.Add(FreeSwitchEvent.ChannelPark);
            e.Add(FreeSwitchEvent.ChannelProgress);
            e.Add(FreeSwitchEvent.ChannelState);
            e.Add(FreeSwitchEvent.ChannelUnbridge);
            e.Add(FreeSwitchEvent.ChannelUnpark);
            return e;
        }

        public static FreeSwitchEventCollection GetAll()
        {
            var e = new FreeSwitchEventCollection();
            e.Add(FreeSwitchEvent.All);
            return e;
        }
    }
}