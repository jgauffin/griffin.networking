using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.Networking.Http.Server
{

    /// <summary>
    /// A HTTP module which do something useful with the request.
    /// </summary>
    public interface IWorkerModule : IHttpModule
    {
        /// <summary>
        /// Handle the request.
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <returns><see cref="ModuleResult.Stop"/> will stop all processing except <see cref="IHttpModule.EndRequest"/>.</returns>
        /// <remarks>Invoked in turn for all modules unless you return <see cref="ModuleResult.Stop"/>.</remarks>
        ModuleResult HandleRequest(IHttpContext context);
    }
}
