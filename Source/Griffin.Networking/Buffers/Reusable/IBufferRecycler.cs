namespace Griffin.Networking.Buffers.Reusable
{
    /// <summary>
    /// A class which can reuse buffers which are released (the parent class gets disposed).
    /// </summary>
    public interface IBufferRecycler
    {
        /// <summary>
        /// Recycle the buffer that the slice contains
        /// </summary>
        /// <param name="slize">Slice which buffer are avaiable for recyling.</param>
        void Recycle(BufferSlize slize);
    }
}