namespace Griffin.Networking.Buffers
{
    /// <summary>
    /// We wrap a buffer and is therefore giving out information about it.
    /// </summary>
    public interface IBufferWrapper
    {
        /// <summary>
        /// Gets or sets current position in the buffer
        /// </summary>
        int Position { get; set; }

        /// <summary>
        /// Gets number of bytes currently written into the buffer
        /// </summary>
        int Count { get;  }

        /// <summary>
        /// Gets the capactiy of the buffer
        /// </summary>
        int Capacity { get;  }
    }
}