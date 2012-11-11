using System.IO;

namespace Griffin.Networking.Buffers
{
    /// <summary>
    /// Write information into a buffer
    /// </summary>
    public interface IBufferWriter : IBufferWrapper
    {
        /// <summary>
        /// Write something to us from the specified buffer.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies <paramref name="count" /> bytes from <paramref name="buffer" /> to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <exception cref="System.ArgumentNullException">buffer</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">offset;count;</exception>
        void Write(byte[] buffer, int offset, int count);

        /// <summary>
        /// Copy everything from the specified stream into this writer.
        /// </summary>
        /// <param name="stream">Stream to copy information from.</param>
        void Copy(Stream stream);
    }
}