using System;

namespace Griffin.Networking.Protocol.FreeSwitch
{
    public class CallerId
    {
        public static readonly CallerId Empty = new CallerId("", new PlainNumber(""));
        private readonly string _name;
        private readonly IPhoneNumber _number;

        public CallerId(string name, IPhoneNumber number)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (number == null) throw new ArgumentNullException("number");
            _name = name;
            _number = number;
        }

        public IPhoneNumber Number
        {
            get { return _number; }
        }

        public string Name
        {
            get { return _name; }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Name) && (Number == null || Number.ToString() == ""))
                return "";

            return string.IsNullOrEmpty(Name) ? Number.ToString() : string.Format(@"<""{0}"" {1}>", Name, Number);
        }
    }

    public interface IPhoneNumber
    {
    }

    public interface ISwitchUser
    {
    }
}