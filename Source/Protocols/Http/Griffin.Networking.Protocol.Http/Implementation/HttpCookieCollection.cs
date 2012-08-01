using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Networking.Http.Specification;

namespace Griffin.Networking.Http.Implementation
{
    class HttpCookieCollection<T> : IHttpCookieCollection<T> where T : IHttpCookie
    {
        List<T> _items = new List<T>();

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
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
        /// Gets the count of cookies in the collection.
        /// </summary>
        public int Count
        {
            get { return _items.Count; }
        }

        /// <summary>
        /// Gets the cookie of a given identifier (<c>null</c> if not existing).
        /// </summary>
        public T this[string id]
        {
            get { return _items.FirstOrDefault(x => x.Name.Equals(id, StringComparison.OrdinalIgnoreCase)); }
        }

        /// <summary>
        /// Remove all cookies.
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }

        /// <summary>
        /// Remove a cookie from the collection.
        /// </summary>
        /// <param name="cookieName">Name of cookie.</param>
        public void Remove(string cookieName)
        {
            _items.RemoveAll(x => x.Name.Equals(cookieName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
