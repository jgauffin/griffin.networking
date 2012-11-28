using System;
using System.Collections.Generic;
using System.Linq;
using Griffin.Networking.Protocol.Http.Services.Routing;

namespace Griffin.Networking.Protocol.Http.Server.Modules
{
    /// <summary>
    /// Used to route the request.
    /// </summary>
    /// <remarks>You may either simply rewrite the request URI or by using a response redirect.</remarks>
    public class RouterModule : IRoutingModule
    {
        private readonly List<IRequestRouter> _routers = new List<IRequestRouter>();

        #region IRoutingModule Members

        /// <summary>
        /// Invoked before anything else
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <remarks>
        /// <para>The first method that is exeucted in the pipeline.</para>
        /// Try to avoid throwing exceptions if you can. Let all modules have a chance to handle this method. You may break the processing in any other method than the Begin/EndRequest methods.</remarks>
        public void BeginRequest(IHttpContext context)
        {
        }

        /// <summary>
        /// End request is typically used for post processing. The response should already contain everything required.
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <remarks>
        /// <para>The last method that is executed in the pipeline.</para>
        /// Try to avoid throwing exceptions if you can. Let all modules have a chance to handle this method. You may break the processing in any other method than the Begin/EndRequest methods.</remarks>
        public void EndRequest(IHttpContext context)
        {
        }

        /// <summary>
        /// Route the request.
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <returns><see cref="ModuleResult.Stop"/> will stop all processing including <see cref="IHttpModule.EndRequest"/>.</returns>
        /// <remarks>Simply change the request URI to something else.</remarks>
        public ModuleResult Route(IHttpContext context)
        {
            return _routers.Any(router => router.Route(context)) ? ModuleResult.Stop : ModuleResult.Continue;
        }

        #endregion

        /// <summary>
        /// Add a new router
        /// </summary>
        /// <param name="router">Router</param>
        public void Add(IRequestRouter router)
        {
            if (router == null) throw new ArgumentNullException("router");
            _routers.Add(router);
        }
    }
}