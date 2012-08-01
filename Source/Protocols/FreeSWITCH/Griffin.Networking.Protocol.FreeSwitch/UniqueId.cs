namespace Griffin.Networking.Protocol.FreeSwitch
{
    public class UniqueId
    {
        private readonly string _id;

        public UniqueId(string id)
        {
            _id = id;
        }

        public override string ToString()
        {
            return _id;
        }

        public override bool Equals(object obj)
        {
            var item = obj as UniqueId;
            if (item == null)
                return false;

            return _id == item._id;
        }

        public bool Equals(UniqueId other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._id, _id);
        }

        public override int GetHashCode()
        {
            return (_id != null ? _id.GetHashCode() : 0);
        }
    }
}