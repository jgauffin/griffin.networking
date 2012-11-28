using System;
using System.Collections.Generic;
using System.Linq;
using Griffin.Networking.Protocol.Http.Server;

namespace Griffin.Networking.Protocol.Http.Services.Routing
{
    /// <summary>
    /// Uses named regular expressions to identify parameters
    /// </summary>
    public class RegexRouter : IRequestRouter
    {
        private readonly List<RegexPattern> _patterns = new List<RegexPattern>();

        #region IRequestRouter Members

        /// <summary>
        /// Route the request.
        /// </summary>
        /// <param name="context">HTTP context used to identify the route</param>
        /// <returns><c>true</c> if we generated some routing; otherwise <c>false</c></returns>
        public bool Route(IHttpContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            return _patterns.Any(pattern => pattern.Match(context));
        }

        #endregion

        /// <summary>
        /// Add a regex and default values
        /// </summary>
        /// <param name="pattern">Pattern. A typical MVC pattern is <![CDATA[@"/(?<controller>[^/]+)/(?<action>[^/]+)?/(?<action>[^/]+).*"]]>.</param>
        /// <param name="defaults">For MVC it would be <c>new { controller = "home", action = "index", id = null }</c></param>
        public void Add(string pattern, object defaults)
        {
            if (pattern == null) throw new ArgumentNullException("pattern");
            if (defaults == null) throw new ArgumentNullException("defaults");

            _patterns.Add(new RegexPattern(pattern, defaults));
        }
    }
}