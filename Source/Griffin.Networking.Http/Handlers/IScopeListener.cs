namespace Griffin.Networking.Http.Handlers
{
    /// <summary>
    /// Implemented by the inversion of control container adapter to create child containers.
    /// </summary>
    public interface IScopeListener
    {
        /// <summary>
        /// A request scope should be created
        /// </summary>
        /// <param name="id">ID identifying the scope</param>
        void ScopeStarted(object id);

        /// <summary>
        /// A scope haver ended.
        /// </summary>
        /// <param name="id">Same id as in <see cref="ScopeStarted"/></param>
        void ScopeEnded(object id);
    }
}