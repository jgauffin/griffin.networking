using System;
using System.IO;
using Griffin.Networking.Protocol.Http.Server;

namespace Griffin.Networking.Protocol.Http.Services.ViewEngines
{
    /// <summary>
    /// Context used to identify view etc.
    /// </summary>
    public class ViewContext
    {
        /// <summary>
        /// Gets or sets path to the requested view.
        /// </summary>
        /// <remarks>Absolute path (starting with slash) means that the specified view should be used and nothing else. Relative paths means that the <c>Shared</c> path will also be checked. The file extension
        /// should never be included.</remarks>
        public string ViewPath { get; set; }

        /// <summary>
        /// The view should be written to this writer.
        /// </summary>
        public TextWriter ViewWriter { get; set; }

        /// <summary>
        /// Model (if specified)
        /// </summary>
        public object Model { get; set; }

        /// <summary>
        /// Model type
        /// </summary>
        public Type ModuleType { get; set; }

        /// <summary>
        /// Additional view data
        /// </summary>
        public IItemStorage ViewData { get; set; }
    }
}