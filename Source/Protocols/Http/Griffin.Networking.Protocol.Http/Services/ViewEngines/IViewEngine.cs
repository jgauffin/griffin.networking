namespace Griffin.Networking.Protocol.Http.Services.ViewEngines
{
    /// <summary>
    /// A view engine
    /// </summary>
    /// <remarks><para>View engines should not try to resolve views in other paths than the specified one.</para></remarks>
    public interface IViewEngine
    {
        /// <summary>
        /// Render view
        /// </summary>
        /// <param name="context">Context information</param>
        /// <returns><c>true</c> if this engine rendered the view; otherwise <c>null</c>.</returns>
        bool Render(ViewEngineContext context);
    }
}