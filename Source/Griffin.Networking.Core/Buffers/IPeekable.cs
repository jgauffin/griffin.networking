namespace Griffin.Networking.Buffers
{
    /// <summary>
    /// Can peek at the next byte without moving forward.
    /// </summary>
    public interface IPeekable
    {
        /// <summary>
        /// Peek at the next byte in the sequence.
        /// </summary>
        /// <returns>Char if not EOF; otherwise <see cref="char.MinValue"/></returns>
        char Peek();
    }
}