using Griffin.Networking.Http.Protocol;

namespace Griffin.Networking.Http.Services.Authentication
{
    /// <summary>
    /// Context for <see cref="IPrincipalFactory"/>.
    /// </summary>
    public class PrincipalFactoryContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrincipalFactoryContext" /> class.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="user">The user.</param>
        public PrincipalFactoryContext(IRequest request, IAuthenticationUser user)
        {
            User = user;
            Request = request;
        }

        /// <summary>
        /// Gets the user which was provided by the <see cref="IAccountStorage"/>.
        /// </summary>
        public IAuthenticationUser User { get; private set; }

        /// <summary>
        /// Gets the HTTP request.
        /// </summary>
        public IRequest Request { get; private set; }
    }
}