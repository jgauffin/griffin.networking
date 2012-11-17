using System;
using Griffin.Networking.Http.Services.ViewEngines;

namespace Griffin.Networking.Http.Razor
{
    /// <summary>
    /// 
    /// </summary>
    public class RazorViewEngine : IViewEngine
    {
        public Type ViewBaseType { get; set; }

        #region IViewEngine Members

        /// <summary>
        /// Render view
        /// </summary>
        /// <param name="context">Context information</param>
        /// <returns><c>true</c> if this engine rendered the view; otherwise <c>null</c>.</returns>
        public bool Render(ViewEngineContext context)
        {
            return false;
        }

        #endregion
    }
}