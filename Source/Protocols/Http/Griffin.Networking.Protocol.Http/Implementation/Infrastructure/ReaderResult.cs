namespace Griffin.Networking.Protocol.Http.Implementation.Infrastructure
{
    /// <summary>
    /// Result from <see cref="TextReaderExtensions.ReadToEnd"/>
    /// </summary>
    public class ReaderResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReaderResult" /> class.
        /// </summary>
        public ReaderResult()
        {
            Value = "";
        }

        /// <summary>
        /// Value read
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Found delimiter
        /// </summary>
        public char Delimiter { get; set; }
    }
}