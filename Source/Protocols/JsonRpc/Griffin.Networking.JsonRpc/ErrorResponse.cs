using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Griffin.Networking.JsonRpc
{
    /// <summary>
    /// Return an error
    /// </summary>
    public class ErrorResponse : ResponseBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorResponse"/> class.
        /// </summary>
        public ErrorResponse(object id) : base(id)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorResponse"/> class.
        /// </summary>
        /// <param name="error">The error.</param>
        public ErrorResponse(object id, RpcError error)
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
