using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Griffin.Networking.JsonRpc
{
    /// <summary>
    /// Response base class
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        /// <param name="id">Id from the request</param>
        protected Response(string id)
        {
            if (id == null) throw new ArgumentNullException("id");
            Id = id;
            JsonRpc = "2.0";
        }

        ///<summary>
        ///  Gets or sets an identifier which was used in the request.
        ///</summary>
        ///<remarks>
        ///  <para>MUST contain a String, Number, or NULL value if included. If it is not included it is assumed to be a notification. The value SHOULD normally not be Null [1] and Numbers SHOULD NOT contain fractional parts [2].</para> <para>The Server MUST reply with the same value in the Response object if included. This member is used to correlate the context between the two objects.</para> <para>[1] The use of Null as a value for the id member in a Request object is discouraged, because this specification uses a value of Null for Responses with an unknown id. Also, because JSON-RPC 1.0 uses an id value of Null for Notifications this could cause confusion in handling.</para> <para>[2] Fractional parts may be problematic, since many decimal fractions cannot be represented exactly as binary fractions.</para>
        ///</remarks>
        [DataMember(Name = "id", IsRequired = false)]
        public string Id { get; set; }

        /// <summary>
        ///   Gets or sets a string specifying the version of the JSON-RPC protocol.
        /// </summary>
        /// <remarks>
        ///   MUST be exactly "2.0".
        /// </remarks>
        [DataMember(Name = "jsonrpc", IsRequired = true)]
        public string JsonRpc { get; set; }
    }

    class SuccessfulResponse : Response
    {
        public SuccessfulResponse(string id, object value)
            : base(id)
        {
            Result = value;
        }

        [DataMember(Name="result", IsRequired = true)]
        public object Result { get; set; }
    }

    /// <summary>
    /// Return an error
    /// </summary>
    public class ErrorResponse : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorResponse"/> class.
        /// </summary>
        public ErrorResponse(string id) : base(id)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorResponse"/> class.
        /// </summary>
        /// <param name="error">The error.</param>
        public ErrorResponse(string id, RpcError error)
            : base(id)
        {
            Error = error;
        }


        [DataMember(Name = "error", IsRequired = true)]
        public RpcError Error { get; protected set; }
    }

//{"jsonrpc": "2.0", "result": 19, "id": 3}
    //{"jsonrpc": "2.0", "error": {"code": -32601, "message": "Method not found."}, "id": "1"}
}
