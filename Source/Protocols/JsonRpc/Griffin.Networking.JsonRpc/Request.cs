using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Griffin.Networking.JsonRpc
{
    ///<summary>
    ///  A rpc call is represented by sending a Request object to a Server
    ///</summary>
    ///<remarks>
    ///  <para>A Notification is a Request object without an "id" member. A Request object that is a Notification signifies the Client's lack of interest in the corresponding Response object, and as such no Response object needs to be returned to the client. The Server MUST NOT reply to a Notification, including those that are within a batch request.</para> <para>Notifications are not confirmable by definition, since they do not have a Response object to be returned. As such, the Client would not be aware of any errors (like e.g. "Invalid params.","Internal error.").</para>
    ///</remarks>
    [DataContract]
    public class Request
    {
        ///<summary>
        ///  Gets or sets an identifier established by the Client.
        ///</summary>
        ///<remarks>
        ///  <para>MUST contain a String, Number, or NULL value if included. If it is not included it is assumed to be a notification. The value SHOULD normally not be Null [1] and Numbers SHOULD NOT contain fractional parts [2].</para> <para>The Server MUST reply with the same value in the Response object if included. This member is used to correlate the context between the two objects.</para> <para>[1] The use of Null as a value for the id member in a Request object is discouraged, because this specification uses a value of Null for Responses with an unknown id. Also, because JSON-RPC 1.0 uses an id value of Null for Notifications this could cause confusion in handling.</para> <para>[2] Fractional parts may be problematic, since many decimal fractions cannot be represented exactly as binary fractions.</para>
        ///</remarks>
        [DataMember(Name = "id", IsRequired = false)]
        public object Id { get; set; }

        /// <summary>
        ///   Gets or sets a string specifying the version of the JSON-RPC protocol.
        /// </summary>
        /// <remarks>
        ///   MUST be exactly "2.0".
        /// </remarks>
        [DataMember(Name = "jsonrpc", IsRequired = true)]
        public string JsonRpc { get; set; }

        /// <summary>
        ///   Gets or sets a string containing the name of the method to be invoked
        /// </summary>
        /// <remarks>
        ///   Method names that begin with the word rpc followed by a period character (U+002E or ASCII 46) are reserved for rpc-internal methods and extensions and MUST NOT be used for anything else.
        /// </remarks>
        [DataMember(Name = "method", IsRequired = true)]
        public string Method { get; set; }

        /// <summary>
        ///   Gets or sets a structured value that holds the parameter values to be used during the invocation of the method
        /// </summary>
        /// <remarks>
        ///   This member MAY be omitted.
        /// </remarks>
        [DataMember(Name = "params", IsRequired = false)]
        [JsonConverter(typeof(ArgumentConverter))]
        public object[] Parameters { get; set; }
    }
}