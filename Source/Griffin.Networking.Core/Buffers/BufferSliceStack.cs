using System;
using System.Collections.Concurrent;

namespace Griffin.Networking.Buffers
{
    /// <summary>
    /// A stack of buffer slices
    /// </summary>
    /// <remarks>Allocates a large buffer which then will be sliced into the number of specified pieces.</remarks>
    /// <seealso cref="PooledBufferSlice"/>
    public class BufferSliceStack : IBufferSliceStack
    {
        private readonly byte[] _buffer;
        private readonly int _numberOfBuffers;
        private readonly ConcurrentStack<PooledBufferSlice> _slices = new ConcurrentStack<PooledBufferSlice>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferSliceStack" /> class.
        /// </summary>
        /// <param name="numberOfBuffers">The number of buffers thar we should support.</param>
        /// <param name="bufferSize">Number of bytes for one slice.</param>
        public BufferSliceStack(int numberOfBuffers, int bufferSize)
        {
            if (numberOfBuffers <= 0)
                throw new ArgumentOutOfRangeException("numberOfBuffers", numberOfBuffers, "Must be 1 or larger.");
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException("bufferSize", bufferSize, "Must be 1 or larger.");

            _numberOfBuffers = numberOfBuffers;
            _buffer = new byte[numberOfBuffers*bufferSize];
            for (var i = 0; i < numberOfBuffers; i++)
            {
                _slices.Push(new PooledBufferSlice(this, _buffer, i*bufferSize, bufferSize));
            }
        }

        #region IBufferSliceStack Members

        /// <summary>
        /// Pop a slice from the stack
        /// </summary>
        /// <returns>A popped slice</returns>
        /// <exception cref="System.InvalidOperationException">No more slices are available (dont forget to return them by disposing the buffers)</exception>
        public IBufferSlice Pop()
        {
            PooledBufferSlice slice;
            if (_slices.TryPop(out slice))
                return slice;

            throw new InvalidOperationException(string.Format("All {0} has been given out.", _numberOfBuffers));
        }

        /// <summary>
        /// Gives back a slice.
        /// </summary>
        /// <param name="slice">The slice.</param>
        /// <exception cref="System.InvalidOperationException">We did not give you away, hence we can't take you. Find your real stack.</exception>
        void IBufferSliceStack.Push(IBufferSlice slice)
        {
            if (slice == null) throw new ArgumentNullException("slice");
            var mySlice = slice as PooledBufferSlice;
            if (mySlice == null || !mySlice.IsMyStack(this))
                throw new InvalidOperationException(
                    "We did not give you away, hence we can't take you. Find your real stack.");

            mySlice.Reset();
            _slices.Push(mySlice);
        }

        #endregion
    }
}