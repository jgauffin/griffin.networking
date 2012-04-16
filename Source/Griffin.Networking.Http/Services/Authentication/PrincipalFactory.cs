using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Griffin.Networking.Http.Protocol;

namespace Griffin.Networking.Http.Services.Authentication
{
    /// <summary>
    /// Used to create <see cref="IPrincipal"/>
    /// </summary>
    public interface IPrincipalFactory
    {
        /// <summary>
        /// Create a new prinicpal
        /// </summary>
        /// <param name="context">Context used to identify the user.</param>
        /// <returns>Principal to use</returns>
        IPrincipal Create(PrincipalFactoryContext context);
    }

    public class PrincipalFactoryContext
    {
        public IAuthenticationUser User { get; set; }
        public IRequest Request { get; set; }
    }
}
