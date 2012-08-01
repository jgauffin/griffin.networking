namespace Griffin.Networking.Protocol.FreeSwitch.CallManager
{
    public interface ICall
    {
    }

    public class CallId
    {
        private readonly string _id;

        public CallId(string id)
        {
            _id = id;
        }
    }
}