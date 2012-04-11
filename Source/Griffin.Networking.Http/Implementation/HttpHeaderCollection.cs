using System;
using System.Collections;
using System.Collections.Generic;
using Griffin.Networking.Http.Protocol;

namespace Griffin.Networking.Http.Implementation
{
    internal class HttpHeaderCollection : IHeaderCollection
    {
        readonly Dictionary<string, IHeader>  _items = new Dictionary<string, IHeader>(StringComparer.OrdinalIgnoreCase);

        public void Add(string name, string value)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (value == null) throw new ArgumentNullException("value");
            _items.Add(name, new HttpHeader(name, value));
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<IHeader> GetEnumerator()
        {
            return _items.Values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets a header
        /// </summary>
        /// <param name="name">header name.</param>
        /// <returns>value if found; otherwise <c>null</c>.</returns>
        public IHeader this[string name]
        {
            get
            {
                IHeader header;
                return _items.TryGetValue(name, out header) ? null : header;
            }
            set { _items[name] = value; }
        }
    }
}