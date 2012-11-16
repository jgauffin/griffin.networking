namespace Griffin.Networking.Http.Server
{
    /// <summary>
    /// Abstraction used for different storages.
    /// </summary>
    public interface IItemStorage
    {
        /// <summary>
        /// Get or set an item
        /// </summary>
        /// <param name="name">Case insensitive name</param>
        /// <returns>Item if found; otherwise <c>null</c>.</returns>
        object this[string name] { get; set; }
    }
}