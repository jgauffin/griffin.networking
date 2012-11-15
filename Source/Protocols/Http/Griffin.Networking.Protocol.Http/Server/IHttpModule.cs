using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.Networking.Http.Server
{
    /// <summary>
    /// A http module
    /// </summary>
    /// <remarks>Will first invoke BeginRequest in all methods and then EndRequest in all modules. <para>Abort
    /// means only that the current method will be aborted for the modules. (EndRequest will still be invoked if you trigger
    /// abort in the BeginRequest)</para></remarks>
    public interface IHttpModule
    {
        /// <summary>
        /// Invoked before anything else
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <returns><see cref="ModuleResult.Stop"/> will stop all processing except <see cref="EndRequest"/>.</returns>
        /// <remarks>First method to get executed in the pipeline. Invoked in turn for all modules unless you return <see cref="ModuleResult.Stop"/>.</remarks>
        ModuleResult BeginRequest(IRequestContext context);

        /// <summary>
        /// Handle the request.
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <returns><see cref="ModuleResult.Stop"/> will stop all processing except <see cref="EndRequest"/>.</returns>
        /// <remarks>Invoked in turn for all modules unless you return <see cref="ModuleResult.Stop"/>.</remarks>
        ModuleResult HandleRequest(IRequestContext context);

        /// <summary>
        /// End request is typically used for post processing. The response should already contain everything required.
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <remarks>The last method to get executed in the request pipeline.</remarks>
        void EndRequest(IRequestContext context);
    }
}
