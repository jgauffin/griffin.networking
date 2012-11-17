using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Griffin.Networking.Http.Server;

namespace Griffin.Networking.Http.Services.Routing
{
    /// <summary>
    /// Used to match a single pattern
    /// </summary>
    public class RegexPattern
    {
        private Regex _regex;
        private IDictionary<string, object> _defaults;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexPattern" /> class.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="defaults">The defaults.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public RegexPattern(string pattern, object defaults)
        {
            if (pattern == null) throw new ArgumentNullException("pattern");
            if (defaults == null) throw new ArgumentNullException("defaults");

            _regex = new Regex(pattern);
            var dyn = (dynamic)defaults;
            _defaults = (IDictionary<string, object>)dyn;
        }

        /// <summary>
        /// Match the route and apply the context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool Match(IHttpContext context)
        {
            var match = _regex.Match(context.Request.Uri.AbsolutePath);
            if (!match.Success)
                return false;


            throw new NotImplementedException("This class is not done.");
        }
    }
}