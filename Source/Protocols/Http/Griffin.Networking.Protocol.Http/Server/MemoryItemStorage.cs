using System;
using System.Collections.Generic;

namespace Griffin.Networking.Http.Server
{
    /// <summary>
    /// Uses a Dictionary to store all items
    /// </summary>
    public class MemoryItemStorage : IItemStorage
    {
        private readonly Dictionary<string, object> _dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Get or set an item
        /// </summary>
        /// <param name="name">Item name</param>
        /// <returns>Item if found; otherwise <c>null</c>.</returns>
        public object this[string name]
        {
            get
            {
                if (name == null)
                    throw new ArgumentNullException("name");
                object value;
                return _dictionary.TryGetValue(name, out value) ? value : null;
            }
            set
            {
                if (name == null)
                    throw new ArgumentNullException("name");

                if (value == null)
                    _dictionary.Remove(name);
                else
                    _dictionary[name] = value;
            }
        }
    }
}