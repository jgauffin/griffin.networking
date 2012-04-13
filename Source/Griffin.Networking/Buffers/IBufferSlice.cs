namespace Griffin.Networking.Buffers
{
    /// <summary>
    /// A slice in a byte[] buffer.
    /// </summary>
    public interface IBufferSlice
    {
        /// <summary>
        /// Gets buffer that this slice is int
        /// </summary>
        byte[] Buffer { get; }

        /// <summary>
        /// Gets offset for this allocated slice
        /// </summary>
        int StartOffset { get; }

        /// <summary>
        /// Gets assign size for this slice
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// Gets current offset in the slice (offset in whole buffer)
        /// </summary>
        int Position { get; set; }

        /// <summary>
        /// Gets number of bytes written to this slice.
        /// </summary>
        int Count { get; set; }
    }
}