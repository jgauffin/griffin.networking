namespace Griffin.Networking.Buffers
{
    /// <summary>
    /// We are a part of a larger buffer
    /// </summary>
    /// <remarks>It's important that you check if a buffer implement <code>IDisposable</code> since you then have
    /// to invoke <c>Dispose()</c> when done to return the buffer to the pool.</remarks>
    public interface IBufferSlice
    {
        /// <summary>
        /// Gets buffer that we are a part of
        /// </summary>
        byte[] Buffer { get; }

        /// <summary>
        /// Gets start index for this slice
        /// </summary>
        int Offset { get;  }

        /// <summary>
        /// Number of bytes allocated for this slice.
        /// </summary>
        int Count { get; }
    }
}