using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.Networking.Http.Implementation
{
    class HttpCookie : IHttpCookie
    {
        /// <summary>
        /// Gets the cookie identifier.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets value. 
        /// </summary>
        /// <remarks>
        /// Set to <c>null</c> to remove cookie.
        /// </remarks>
        public string Value { get; set; }
    }
}
