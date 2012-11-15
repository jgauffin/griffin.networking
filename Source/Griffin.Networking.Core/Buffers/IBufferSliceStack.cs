namespace Griffin.Networking.Buffers
{
    /// <summary>
    /// A stack of buffer slices (i.e. a pool)
    /// </summary>
    public interface IBufferSliceStack
    {
        /// <summary>
        /// Return a buffer slice
        /// </summary>
        /// <param name="slice">Slice</param>
        /// <remarks>The slice MUST have come from this pool.</remarks>
        void Push(IBufferSlice slice);

        /// <summary>
        /// Pop a slice.
        /// </summary>
        /// <returns>Slice given out</returns>
        IBufferSlice Pop();
    }
}