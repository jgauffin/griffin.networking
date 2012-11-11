using System;
using System.IO;

namespace Griffin.Networking.Buffers
{
    /// <summary>
    /// Read from a buffer into a stream
    /// </summary>
    public interface IBufferReader : IBufferWrapper
    {
        /// <summary>
        /// Write our information into another buffer
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset" /> and (<paramref name="offset" /> + <paramref name="count" /> - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">buffer</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">offset;count</exception>
        int Read(byte[] buffer, int offset, int count);

        /// <summary>
        /// Read one byte
        /// </summary>
        /// <returns>Byte if read; -1 if end of stream.</returns>
        int Read();

        /// <summary>
        /// Write our contents to another stream
        /// </summary>
        /// <param name="other">Stream to write to</param>
        /// <param name="count">Number of bytes to write</param>
        /// <returns>Bytes written</returns>
        int CopyTo(Stream other, int count);

        /// <summary>
        /// Gets number of bytes left from the current postion to <see cref="IBufferWrapper.Count"/>.
        /// </summary>
        int RemainingLength { get; }
    }
}