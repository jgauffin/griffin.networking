using Griffin.Networking.Http.Protocol;

namespace Griffin.Networking.Http.Services.Authentication
{
    /// <summary>
    /// Uses <c>request.Uri.Host</c> as realm.
    /// </summary>
    public class HostAsRealmRepository : IRealmRepository
    {
        #region IRealmRepository Members

        /// <summary>
        /// Gets the realm for a request
        /// </summary>
        /// <param name="request">Request which realm we want to get</param>
        /// <returns>The realm that the request belongs to</returns>
        public string GetRealm(IRequest request)
        {
            return request.Uri.Host;
        }

        #endregion
    }
}