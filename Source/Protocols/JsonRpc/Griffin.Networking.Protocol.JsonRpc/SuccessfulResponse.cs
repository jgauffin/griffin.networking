using System.Runtime.Serialization;

namespace Griffin.Networking.JsonRpc
{
    public class Response : ResponseBase
    {
        public Response(object id, object value)
            : base(id)
        {
            Result = value;
        }

        [DataMember(Name="result", IsRequired = true)]
        public object Result { get; set; }
    }
}