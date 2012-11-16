namespace Griffin.Networking.Http.Server
{
    /// <summary>
    /// Takes care of the module execution.
    /// </summary>
    /// <remarks>Will catch all exceptions and also log them including the request information. 
    /// 
    /// It will however not do anything with the exception. You either have to have an error module which checks <see cref="IRequestContext.LastException"/>
    /// in <c>EndRequest()</c> or override the server to handle the error in it.
    /// <para>Modules are invoked in the following order
    /// <list type="number">
    /// <item><see cref="IHttpModule.BeginRequest"/></item>
    /// <item><see cref="IRoutingModule"/></item>
    /// <item><see cref="IAuthenticationModule"/></item>
    /// <item><see cref="IAuthorizationModule"/></item>
    /// <item><see cref="IWorkerModule"/></item>
    /// <item><see cref="IHttpModule.EndRequest"/></item>
    /// </list>
    /// </para>
    /// </remarks>
    public interface IModuleManager
    {
        /// <summary>
        /// Add a HTTP module
        /// </summary>
        /// <param name="module">Module to include</param>
        /// <remarks>Modules are executed in the order they are added.</remarks>
        void Add(IHttpModule module);

        /// <summary>
        /// Invoke all modules
        /// </summary>
        /// <param name="context"></param>
        /// <returns><c>true</c> if no modules have aborted the handling. Any module throwing an exception is also considered to be abort.</returns>
        bool Invoke(IRequestContext context);
    }
}