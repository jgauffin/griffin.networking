namespace Griffin.Networking.Http.Protocol
{
    /// <summary>
    /// A file included in a HTTP request.
    /// </summary>
    public interface IHttpFile
    {
        /// <summary>
        /// Gets or sets content type.
        /// </summary>
        string ContentType { get; set; }

        /// <summary>
        /// Gets or sets name in form.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets name original file name
        /// </summary>
        string OriginalFileName { get; set; }

        /// <summary>
        /// Gets or sets filename for locally stored file.
        /// </summary>
        string TempFileName { get; set; }
    }
}